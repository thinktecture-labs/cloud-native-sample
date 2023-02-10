using System.Data.SqlClient;
using ProductsService.Configuration;

namespace ProductsService.Migrations;

public class Migrations
{
    private readonly ProductsServiceConfiguration _cfg;
    private readonly ILogger<Migrations> _logger;

    public Migrations(ProductsServiceConfiguration cfg, ILogger<Migrations> logger)
    {
        _cfg = cfg;
        _logger = logger;
    }
    private List<IMigration> _FindMigrations()
    {
        return GetType()
        .Assembly
        .GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(IMigration)))
        .Select(t => (IMigration)Activator.CreateInstance(t)!)
        .OrderBy(m => m.Version)
        .ToList();

    }

    public void Migrate()
    {
        using var con = new SqlConnection(_cfg.ConnectionString);
        try
        {
            con.Open();

            _FindMigrations().ForEach(async m => {
                await ExecuteMigrationAsync(con, m);
                await m.PostMigrateAsync(con);
            });
        }
        catch (Exception)
        {
            throw;
        }

    }

    private async Task<int> GetCurrentDatabaseVersion(SqlConnection con)
    {
        using var cmd = new SqlCommand("SELECT Version FROM DatabaseVersion", con);
        try
        {
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                return reader.GetInt32(reader.GetOrdinal("Version"));
            }
            return 0;
        }
        catch (SqlException)
        {
            return 0;
        }

    }

    private async Task ExecuteMigrationAsync(SqlConnection con, IMigration m)
    {
        using var tx = con.BeginTransaction();
        using var cmd = new SqlCommand(m.Script, con);
        cmd.Transaction = tx;
        await cmd.ExecuteNonQueryAsync();
        using var updateVersionCmd = new SqlCommand("UPDATE DatabaseVersion SET Version = @Version", con);
        updateVersionCmd.Parameters.AddWithValue("@Version", m.Version);
        await updateVersionCmd.ExecuteNonQueryAsync();
        try
        {
            tx.Commit();
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;

        }
    }
}
