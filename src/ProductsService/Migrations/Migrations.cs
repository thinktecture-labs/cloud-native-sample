using System.Data.SqlClient;
using ProductsService.Configuration;

namespace ProductsService.Migrations;

// This is a poor-man's migration system. It's not very robust, but it's good enough for this demo.
// In a real-world scenario, you'd probably want to use a proper migration system like FluentMigrator.
public class Migrations
{
    private readonly ProductsServiceConfiguration _cfg;
    private readonly ILogger<Migrations> _logger;

    public Migrations(ProductsServiceConfiguration cfg, ILogger<Migrations> logger)
    {
        _cfg = cfg;
        _logger = logger;
    }
    private List<IMigration> _FindMigrations(int currentVersion)
    {
        return GetType()
        .Assembly
        .GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(IMigration)))
        .Select(t => (IMigration)Activator.CreateInstance(t)!)
        .Where(m => m.Version > currentVersion)
        .OrderBy(m => m.Version)
        .ToList();

    }

    public void Migrate()
    {
        using var con = new SqlConnection(_cfg.ConnectionString);
        try
        {
            con.Open();
            var currentVersion = GetCurrentDatabaseVersion(con);
            
            _FindMigrations(currentVersion).ForEach(m => {
                ExecuteMigration(con, m);
            });
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static int GetCurrentDatabaseVersion(SqlConnection con)
    {
        using var cmd = new SqlCommand("SELECT Version FROM DatabaseVersion");
        cmd.Connection = con;
        try
        {
            var reader = cmd.ExecuteReader();
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

    private void ExecuteMigration(SqlConnection connection, IMigration migration)
    {
        SqlTransaction? transaction = null;
        try
        {
            transaction = connection.BeginTransaction();
            if (!string.IsNullOrWhiteSpace(migration.Script))
            {
                using var cmd = new SqlCommand(migration.Script);
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
            
            migration.PostMigrate(connection, transaction);

            using var updateVersionCmd = new SqlCommand("UPDATE DatabaseVersion SET Version = @Version");
            updateVersionCmd.Connection = connection;
            updateVersionCmd.Transaction = transaction;
            updateVersionCmd.Parameters.AddWithValue("@Version", migration.Version);
            updateVersionCmd.ExecuteNonQuery();
            
            transaction.Commit();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not apply migration {MigrationId}", migration.Version);
            transaction?.Rollback();
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }
}
