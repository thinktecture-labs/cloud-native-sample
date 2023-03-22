using PriceDropNotifier.Data.Model;

namespace PriceDropNotifier.Data.UnitOfWork;

public interface INotificationUnitOfWork
{
    Task<List<PriceWatchRegistration>> GetRegistrationsToBeNotifiedAsync(Guid productId,
                                                                         double newPrice,
                                                                         CancellationToken cancellationToken = default);
}
