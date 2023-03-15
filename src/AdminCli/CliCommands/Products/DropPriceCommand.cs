using System;
using AdminCli.HttpAccess;
using AdminCli.NumberParsing;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Products;

public sealed class DropPriceCommand : ICliCommand
{
    public DropPriceCommand(IHttpService httpService) => HttpService = httpService;

    private IHttpService HttpService { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Drops the price of a specified product.";
        var productIdOption =
            config.Option("-p|--product-id <Product ID>",
                          "The ID of the product whose price should be dropped. Must be a GUID.",
                          CommandOptionType.SingleValue)
                  .IsRequired();
        var dropPriceByOption =
            config.Option("-d|--drop-price-by <Price>",
                          "The amount of money the price should be dropped by. You must specify a decimal number.",
                          CommandOptionType.SingleValue)
                  .IsRequired();
        
        config.OnExecuteAsync(async cancellationToken =>
        {
            if (!Guid.TryParse(productIdOption.Value(), out var parsedId))
            {
                Console.WriteLine($"Could not parse \"{productIdOption.Value()}\" to a GUID.");
                return;
            }

            if (!DoubleParser.TryParse(dropPriceByOption.Value(), out var parsedPriceDrop))
            {
                Console.WriteLine($"Could not parse \"{dropPriceByOption.Value()}\" to a decimal number.");
                return;
            }

            var dto = new PriceDropDto(parsedId, parsedPriceDrop);
            await HttpService.PostAsync("/pricedrops/invoke", dto, cancellationToken);
            Console.WriteLine("Price drop sent successfully");
        });
    }
}

// The properties of this record are read by the JSON serializer
// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record PriceDropDto(Guid ProductId, double DropPriceBy);
// ReSharper restore NotAccessedPositionalProperty.Global
