using System.Data.SqlClient;
using ProductsService.Configuration;
using ProductsService.Data.Entities;

namespace ProductsService.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsServiceConfiguration _cfg;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(ProductsServiceConfiguration cfg, ILogger<ProductsRepository> logger)
        {
            _cfg = cfg;
            _logger = logger;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            using var con = new SqlConnection(_cfg.ConnectionString);
            con.Open();
            var cmd = new SqlCommand("INSERT INTO Products (Id,Name,Description,Tags,Price) VALUES (@Id,@Name,@Description,@Tags,@Price); SELECT SCOPE_IDENTITY()");
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Description", product.Description);
            cmd.Parameters.AddWithValue("@Tags", string.Join(',', product.Categories));
            cmd.Parameters.AddWithValue("@Price", product.Price);
            product.Id = (Guid) await cmd.ExecuteScalarAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var con = new SqlConnection(_cfg.ConnectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT Id,Name,Description,Tags,Price FROM Products");
            cmd.Connection = con;
            var reader = await cmd.ExecuteReaderAsync();
            var products = new List<Product>();
            while (reader.Read())
            {
                var id = reader.GetGuid(reader.GetOrdinal("Id"));
                var name = reader.GetString(reader.GetOrdinal("Name"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var tags = reader.GetString(reader.GetOrdinal("Tags")).Split(',').ToList();
                var price = reader.GetDouble(reader.GetOrdinal("Price"));
                products.Add(new Product(id, name, description, tags, price));
            }
            return products;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            using var con = new SqlConnection(_cfg.ConnectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT Id,Name,Description,Tags,Price FROM Products WHERE Id = @Id");
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Id", id);
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                var name = reader.GetString(reader.GetOrdinal("Name"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var tags = reader.GetString(reader.GetOrdinal("Tags")).Split(',').ToList();
                var price = reader.GetDouble(reader.GetOrdinal("Price"));
                return new Product(id, name, description, tags, price);
            }
            return null;
        }
    }
}
