using System.Data;
using System.Data.SqlClient;
using ProductsService.Data;

namespace ProductsService.Migrations;

// This migration imports the same products into the database
// that are used in in-memory scenarios. This way the price watcher
// service and the product service will use the same product IDs.
// The best solution would be to get the products in the price watcher
// service instead of having its own in-memory collection.
public sealed class ProductsAlignmentMigration : IMigration
{
    public int Version => 2;
    public string Script => "DELETE FROM Products;";
    
    public void PostMigrate(SqlConnection connection, SqlTransaction transaction)
    {
        var products = InMemoryProducts.Products;
        foreach (var product in products)
        {
            using var command = new SqlCommand(
                """
                INSERT INTO Products (Id, Name, Description, Tags, Price)
                VALUES (@Id, @Name, @Description, @Tags, @Price);
                """,
                connection,
                transaction
            );

            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = product.Id;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = product.Name;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = product.Description;
            command.Parameters.Add("@Tags", SqlDbType.VarChar).Value = string.Join(',', product.Categories);
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;

            command.ExecuteNonQuery();
        }
    }
}
