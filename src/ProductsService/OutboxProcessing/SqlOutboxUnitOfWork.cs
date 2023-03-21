using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using ProductsService.Data.Entities;
using ProductsService.Data.UnitOfWork.Sql;

namespace ProductsService.OutboxProcessing;

public sealed class SqlOutboxUnitOfWork : SqlAsyncReadOnlyUnitOfWork, IOutboxUnitOfWork
{
    public SqlOutboxUnitOfWork(SqlConnection connection) : base(connection) { }

    public async Task<List<OutboxItem>> GetNextOutboxItemsAsync(CancellationToken cancellationToken = default)
    {
        await using var command = CreateCommand("SELECT TOP 50 * FROM Outbox ORDER BY CreatedAtUtc;");
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
        return await reader.DeserializeOutboxItemsAsync(cancellationToken);
    }

    public async Task DeleteOutboxItemAsync(OutboxItem outboxItem, CancellationToken cancellationToken)
    {
        await using var command = CreateCommand("DELETE FROM Outbox WHERE Id = @OutboxItemId");
        command.Parameters.Add("@OutboxItemId", SqlDbType.BigInt).Value = outboxItem.Id;
        var result = await command.ExecuteNonQueryAsync(cancellationToken);
        Debug.Assert(result == 1);
    }
}
