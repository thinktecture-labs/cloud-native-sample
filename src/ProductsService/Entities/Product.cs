using System.Diagnostics.CodeAnalysis;

namespace ProductsService.Entities
{
    public class Product
    {
        public Product(Guid id, string name, string description, IEnumerable<string> categories, double price)
        {
            Id = id;
            Name = name;
            Description = description;
            Categories = categories;
            Price = price;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public double Price {get;set;}
    }
}
