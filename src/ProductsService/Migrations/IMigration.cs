using System.Data.SqlClient;

namespace ProductsService.Migrations;

public interface IMigration
{
    int Version { get; }
    string Script {get;}
    Task PostMigrateAsync(SqlConnection con);
}
