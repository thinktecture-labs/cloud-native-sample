using PriceDropNotifier.Data.Model;
using PriceDropNotifier.Data.UnitOfWork;

namespace PriceDropNotifier.Data;

public sealed class InMemoryPriceWatcherContext : IPriceWatcherUnitOfWork,
                                                  INotificationUnitOfWork
{
    private List<PriceWatchRegistration> Registrations { get; } = new ();

    public Task<List<PriceWatchRegistration>> GetRegistrationsToBeNotifiedAsync(Guid productId,
                                                                                double newPrice,
                                                                                CancellationToken cancellationToken) =>
        Task.FromResult(Registrations.Where(r => r.ProductId == productId &&
                                                 r.TargetPrice >= newPrice)
                                     .ToList());


    public Task<PriceWatchRegistration?> GetExistingRegistrationAsync(string email,
                                                                      Guid productId,
                                                                      CancellationToken cancellationToken = default) =>
        Task.FromResult(Registrations.FirstOrDefault(r => r.Email == email &&
                                                          r.ProductId == productId));

    public Task UpdateRegistrationAsync(PriceWatchRegistration registration,
                                        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task InsertRegistrationAsync(PriceWatchRegistration registration,
                                        CancellationToken cancellationToken = default)
    {
        Registrations.Add(registration);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
