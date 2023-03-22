using PriceDropNotifier.Data.Model;

namespace PriceDropNotifier.Data.UnitOfWork;

public interface IPriceWatcherUnitOfWork
{
    Task<PriceWatchRegistration?> GetExistingRegistrationAsync(string email,
                                                               Guid productId,
                                                               CancellationToken cancellationToken = default);

    Task UpdateRegistrationAsync(PriceWatchRegistration registration, CancellationToken cancellationToken = default);
    Task InsertRegistrationAsync(PriceWatchRegistration registration, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
