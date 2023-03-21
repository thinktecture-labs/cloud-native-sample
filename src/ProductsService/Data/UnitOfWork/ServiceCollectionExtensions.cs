namespace ProductsService.Data.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWorkWithFactory<TAbstraction, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime unitOfWorkLifetime = ServiceLifetime.Scoped,
        ServiceLifetime factoryLifetime = ServiceLifetime.Scoped
    )
        where TAbstraction : class, IAsyncReadOnlyUnitOfWork
        where TImplementation : class, TAbstraction, IInitializeAsync
    {
        services.Add(new (typeof(TImplementation), typeof(TImplementation), unitOfWorkLifetime));
        services.Add(new (typeof(IAsyncFactory<TAbstraction>),
                          typeof(AsyncFactory<TAbstraction, TImplementation>),
                          factoryLifetime));
        services.Add(new (typeof(Func<TImplementation>),
                          serviceProvider =>
                              new Func<TImplementation>(serviceProvider.GetRequiredService<TImplementation>),
                          factoryLifetime));
        return services;
    }
}
