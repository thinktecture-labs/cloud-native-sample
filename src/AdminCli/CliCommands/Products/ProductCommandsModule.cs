using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.CliCommands.Products;

public static class ProductCommandsModule
{
    public static IServiceCollection AddProductCommands(this IServiceCollection services) =>
        services.AddSingleton<ProductsCommand>()
                .AddSingleton<ListProductsCommand>()
                .AddSingleton<DropPriceCommand>();
}
