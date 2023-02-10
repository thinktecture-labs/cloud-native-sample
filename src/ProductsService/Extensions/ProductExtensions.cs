using ProductsService.Data.Entities;
using ProductsService.Models;

namespace ProductsService.Extensions;

public static class ProductExtensions
{
    public static Product ToEntity(this ProductCreateModel p)
    {
        return new Product(Guid.Empty, p.Name, p.Description, p.Tags, p.Price);

    }
    public static ProductListModel ToListModel(this Product p)
    {
        return new ProductListModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price
        };
    }

    public static ProductDetailsModel ToDetailsModel(this Product p)
    {
        return new ProductDetailsModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Tags = p.Categories.ToArray()
        };
    }
}
