using System.Data.SqlClient;
using ProductsService.Data.Entities;

namespace ProductsService.Migrations;

public class InitialMigration : IMigration
{
    public int Version => 1;

    public string Script => @"
CREATE TABLE DatabaseVersion (
    Version INT NOT NULL PRIMARY KEY
);

INSERT INTO DatabaseVersion VALUES (" + Version + @");

CREATE TABLE Products (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
  	Name VARCHAR(50) NOT NULL,
  	Description VARCHAR(255) NOT NULL,
  	Tags VARCHAR(255) NOT NULL,
  	Price DECIMAL(2)
);";

    public async void PostMigrate(SqlConnection con)
    {
        var products = new List<Product> {
            new Product (Guid.NewGuid(), "Beer", "Tasty craft beer", new List<string> { "Drinks", "Food" }, 3.99),
            new Product (Guid.NewGuid(), "Whisky", "Gentle drink for cold evenings", new List<string> { "Drinks", "Food" }, 49.99),
            new Product (Guid.NewGuid(), "Bacon Burger", "Everything is better with bacon", new List<string> { "Food" }, 8.99),
        };
        var tx = con.BeginTransaction();
        try
        {
            products.ForEach(p =>
            {
                var cmd = new SqlCommand("INSERT INTO Products (Name, Description, Tags, Price) VALUES (@Name, @Description, @Tags, @Price)");
                cmd.Connection = con;
                cmd.Transaction = tx;
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@Description", p.Description);
                cmd.Parameters.AddWithValue("@Tags", string.Join(',', p.Categories));
                cmd.Parameters.AddWithValue("@Price", p.Price);
                cmd.ExecuteNonQuery();
            });
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
