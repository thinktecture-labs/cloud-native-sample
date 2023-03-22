using System.Data;
using System.Data.SqlClient;

namespace ProductsService.Data.UnitOfWork.Sql;

public abstract class SqlAsyncUnitOfWork : SqlAsyncReadOnlyUnitOfWork, IAsyncUnitOfWork
{
    protected SqlAsyncUnitOfWork(SqlConnection connection,
                                 IsolationLevel? transactionLevel = IsolationLevel.Serializable)
        : base(connection, transactionLevel) { }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Transaction?.Commit();
        return Task.CompletedTask;
    }
}
