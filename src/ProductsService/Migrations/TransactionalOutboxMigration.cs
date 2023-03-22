using System.Data.SqlClient;

namespace ProductsService.Migrations;

public sealed class TransactionalOutboxMigration : IMigration
{
    public int Version => 2;

    public string Script => @"
CREATE TABLE Outbox (
    Id BIGINT IDENTITY(1, 1) CONSTRAINT PK_Outbox PRIMARY KEY CLUSTERED,
    Type NVARCHAR(100) NOT NULL,
    Data NVARCHAR(2048) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Outbox_CreatedAtUtc_Now DEFAULT GETUTCDATE()
);";
    public void PostMigrate(SqlConnection con) { }
}
