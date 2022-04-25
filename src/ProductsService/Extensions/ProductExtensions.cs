// ReSharper disable once CheckNamespace

using ProductsService.Entities;
using ProductsService.Models;

namespace ProductsService;

public static class ProductExtensions
{
    public static ProductListModel ToListModel(this Product p)
    {
        return new ProductListModel
        {
            Id = p.Id,
            Name = p.Name,
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
