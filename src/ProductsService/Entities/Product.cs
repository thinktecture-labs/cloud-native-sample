using System.Diagnostics.CodeAnalysis;

namespace ProductsService.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public String? Name { get; set; }
        public double Price {get;set;}
    }
}
