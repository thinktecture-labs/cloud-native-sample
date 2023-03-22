using Microsoft.AspNetCore.Mvc;
using PriceDropNotifier.Data.Model;
using PriceDropNotifier.Data.UnitOfWork;
using PriceDropNotifier.Models;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("/pricewatcher/register")]
public sealed class PriceWatcherController : ControllerBase
{
    public PriceWatcherController(IPriceWatcherUnitOfWork unitOfWork,
                                  ILogger<PriceWatcherController> logger)
    {
        UnitOfWork = unitOfWork;
        Logger = logger;
    }

    private IPriceWatcherUnitOfWork UnitOfWork { get; }
    private ILogger<PriceWatcherController> Logger { get; }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model, CancellationToken cancellationToken)
    {
        var existingRegistration = await UnitOfWork.GetExistingRegistrationAsync(model.Email,
                                                                                 model.ProductId,
                                                                                 cancellationToken);
        if (existingRegistration is not null)
        {
            var oldPrice = existingRegistration.TargetPrice;
            existingRegistration.TargetPrice = model.Price;
            await UnitOfWork.UpdateRegistrationAsync(existingRegistration, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Existing {Registration} was updated (old price {OldPrice})",
                                  existingRegistration,
                                  oldPrice);
        }
        else
        {
            var newRegistration = PriceWatchRegistration.Create(model.Email, model.ProductId, model.Price);
            await UnitOfWork.InsertRegistrationAsync(newRegistration, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("New {Registration} was created", newRegistration);
        }

        
        return NoContent();
    }
}
