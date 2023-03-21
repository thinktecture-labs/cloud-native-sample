using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Data.Entities;
using ProductsService.Data.Repositories;
using ProductsService.Data.UnitOfWork;
using ProductsService.Data.UnitOfWork.InMemory;
using ProductsService.Data.UnitOfWork.Sql;
using ProductsService.Extensions;
using ProductsService.Models;
using ProductsService.OutboxProcessing;

namespace ProductsService.Controllers;

[ApiController]
[Authorize(AuthPolicies.RequiresAdminScope)]
[Route("/pricedrops/invoke")]
public sealed class PriceDropController : ControllerBase
{
    public PriceDropController(IAsyncFactory<IPriceDropUnitOfWork> unitOfWorkFactory,
                               IOutboxProcessor outboxProcessor,
                               ILogger<PriceDropController> logger)
    {
        UnitOfWorkFactory = unitOfWorkFactory;
        OutboxProcessor = outboxProcessor;
        Logger = logger;
    }

    private IAsyncFactory<IPriceDropUnitOfWork> UnitOfWorkFactory { get; }
    private IOutboxProcessor OutboxProcessor { get; }
    private ILogger<PriceDropController> Logger { get; }

    [HttpPost]
    public async Task<IActionResult> InvokePriceDrop(PriceDropModel model, CancellationToken cancellationToken)
    {
        // UnitOfWorkFactory.CreateAsync will resolve the unit of work and asynchronously open
        // the underlying SqlConnection and SqlTransaction (well, ADO.NET for SQL Server does not
        // support async transactions, but that's another topic). The AsyncFactory implementation also supports
        // scoped lifetimes.
        // I prefer this approach as business logic (well, in this small example it's only
        // product.Price *= 1 - model.DropPriceBy;) is clearly decoupled from I/O. In the other examples
        // in this repo, business logic is mixed with I/O logic in the Repository classes,
        // which results in way more pain when you want to write unit tests.
        // The unit of works are Humble Objects that simply perform I/O and serialize/deserialize outgoing or
        // incoming data.
        await using var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken);
        var product = await unitOfWork.GetProductAsync(model.ProductId, cancellationToken);
        if (product is null)
            return NotFound();

        var previousPrice = product.Price;
        product.Price *= 1 - model.DropPriceBy;
        await unitOfWork.UpdateProductPriceAsync(product, cancellationToken);
        
        // Here we can also see one part of the Transactional Outbox pattern:
        // it might be that committing to the database succeeds, but publishing
        // the event to the message broker does not. In this case, we would have
        // lost a message. To accomodate for this, we save the message to the database
        // so that we can use its transactional capabilities. The OutboxProcessor will
        // then send the messages in an endless loop. This ensures at-least-once semantics.
        // I thought that Dapr would offer at-least-once semantics when publishing events out of the box,
        // but according to this GitHub issue https://github.com/dapr/dapr/issues/4233, it doesn't. The Dapr
        // sidecar does not persist events to be published and relies on the underlying
        // mechanisms of the message broker to ensure its at-least-once semantics claim when sending messages
        // to subscribers. This can be found in the docs:
        // https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/
        await unitOfWork.InsertPriceDropIntoOutbox(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Price was dropped from {PreviousPrice:N2} to {NewPrice:N2} for product {ProductId}",
                              previousPrice,
                              product.Price,
                              product.Id);
        
        OutboxProcessor.StartProcessingIfNecessary();

        return Ok($"Dropped price for {model.ProductId} by {model.DropPriceBy * 100}%.");
    }
}

public interface IPriceDropUnitOfWork : IAsyncUnitOfWork
{
    Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task UpdateProductPriceAsync(Product product, CancellationToken cancellationToken = default);
    Task InsertPriceDropIntoOutbox(Product product, CancellationToken cancellationToken);
}

public sealed class SqlPriceDropUnitOfWork : SqlAsyncUnitOfWork, IPriceDropUnitOfWork
{
    public SqlPriceDropUnitOfWork(SqlConnection connection) : base(connection) { }

    public async Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        await using var command = CreateCommand("SELECT * FROM Products WHERE Id = @ProductId");
        command.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = productId;

        await using var reader = await command.ExecuteReaderAsync(
            CommandBehavior.SingleResult | CommandBehavior.SingleRow,
            cancellationToken
        );
        return await reader.DeserializeProductAsync(cancellationToken);
    }

    public async Task UpdateProductPriceAsync(Product product, CancellationToken cancellationToken = default)
    {
        await using var command = CreateCommand("UPDATE Products SET Price = @NewPrice WHERE Id = @ProductId");
        command.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = product.Id;
        command.Parameters.Add("@NewPrice", SqlDbType.Float).Value = product.Price;

        var numberOfRowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        Debug.Assert(numberOfRowsAffected == 1);
    }

    public async Task InsertPriceDropIntoOutbox(Product product, CancellationToken cancellationToken)
    {
        await using var command = CreateCommand("INSERT INTO Outbox(Type, Data) VALUES (@Type, @Data);");
        var priceDropMessageData = new PriceDropMessageData(product.Id, product.Price);
        var json = JsonSerializer.Serialize(priceDropMessageData);
        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = OutboxItemTypes.PriceDrop;
        command.Parameters.Add("@Data", SqlDbType.NVarChar).Value = json;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

public sealed class InMemoryPriceDropUnitOfWork : InMemoryAsyncUnitOfWork, IPriceDropUnitOfWork
{
    public InMemoryPriceDropUnitOfWork(InMemoryProductsRepository productsRepository) =>
        ProductsRepository = productsRepository;

    private InMemoryProductsRepository ProductsRepository { get; }

    public Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default) =>
        ProductsRepository.GetByIdAsync(productId);

    public Task UpdateProductPriceAsync(Product product, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task InsertPriceDropIntoOutbox(Product product, CancellationToken cancellationToken)
    {
        var data = new PriceDropMessageData(product.Id, product.Price);
        var json = JsonSerializer.Serialize(data);
        var outboxEntry = new OutboxItem(0,
                                         OutboxItemTypes.PriceDrop,
                                         json,
                                         DateTime.UtcNow);
        ProductsRepository.AddOutboxItem(outboxEntry);
        return Task.CompletedTask;
    }
}

public sealed record PriceDropMessageData(Guid ProductId, double Price);
