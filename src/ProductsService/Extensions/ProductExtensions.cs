using ProductsService.Data.Entities;
using ProductsService.Models;

namespace ProductsService.Extensions;

public static class ProductExtensions
{
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
            Categories = p.Categories.ToArray()
        };
    }
}
