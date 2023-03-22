using System.Data;
using System.Data.SqlClient;

namespace ProductsService.Data.UnitOfWork.Sql;

public abstract class SqlAsyncReadOnlyUnitOfWork : IAsyncReadOnlyUnitOfWork, IInitializeAsync
{
    protected SqlAsyncReadOnlyUnitOfWork(SqlConnection connection, IsolationLevel? transactionLevel = null)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        TransactionLevel = transactionLevel;
    }

    protected SqlConnection Connection { get; }
    protected SqlTransaction? Transaction { get; private set; }
    protected IsolationLevel? TransactionLevel { get; }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
            await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection.Dispose();
    }
    
    protected bool IsInitialized { get; private set; }

    bool IInitializeAsync.IsInitialized => IsInitialized;

    async Task IInitializeAsync.InitializeAsync(CancellationToken cancellationToken)
    {
        if (IsInitialized)
            return;

        await Connection.OpenAsync(cancellationToken);

        if (TransactionLevel.HasValue)
            Transaction = Connection.BeginTransaction(TransactionLevel.Value);
        
        IsInitialized = true;
    }

    protected SqlCommand CreateCommand(string? sql = null, CommandType commandType = CommandType.Text)
    {
        var sqlCommand = Connection.CreateCommand();
        if (Transaction is not null)
            sqlCommand.Transaction = Transaction;
        if (!string.IsNullOrWhiteSpace(sql))
            sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        return sqlCommand;
    }
}
