using System;
using System.Collections.Generic;
using AdminCli.HttpAccess;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Products;

public sealed class ListProductsCommand : ICliCommand
{
    public ListProductsCommand(IHttpService httpService)
    {
        HttpService = httpService;
    }

    private IHttpService HttpService { get; }


    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Lists all available products.";
        config.OnExecuteAsync(async cancellationToken =>
        {
            var products = await HttpService.GetAsync<List<ProductListModel>>("/products", cancellationToken);
            if (products is null || products.Count == 0)
            {
                Console.WriteLine("No products were found");
                return;
            }
            
            foreach (var product in products)
            {
                Console.Write("Id: ");
                Console.WriteLine(product.Id.ToString());
                
                Console.Write("Name: ");
                Console.WriteLine(product.Name);
                
                Console.Write("Price: ");
                Console.WriteLine(product.Price.ToString("C2"));
                
                Console.WriteLine(product.Description);
                Console.WriteLine();
            }
        });
    }
}

// This class is instantiated and populated by JSON deserialization,
// which is why we disable the following ReSharper rules.
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class ProductListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
}
// ReSharper restore UnusedAutoPropertyAccessor.Global
