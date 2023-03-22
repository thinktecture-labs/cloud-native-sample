using PriceDropNotifier.Data.UnitOfWork;

namespace PriceDropNotifier.Data;

public static class DataModule
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services) =>
        services.AddSingleton<InMemoryPriceWatcherContext>()
                .AddSingleton<IPriceWatcherUnitOfWork>(sp => sp.GetRequiredService<InMemoryPriceWatcherContext>())
                .AddSingleton<INotificationUnitOfWork>(sp => sp.GetRequiredService<InMemoryPriceWatcherContext>());
}
