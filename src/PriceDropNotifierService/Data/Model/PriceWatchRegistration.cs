namespace PriceDropNotifier.Data.Model;

public sealed class PriceWatchRegistration
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public double TargetPrice { get; set; }

    public static PriceWatchRegistration Create(string email, Guid productId, double targetPrice) =>
        new ()
        {
            Id = Guid.NewGuid(),
            Email = email,
            ProductId = productId,
            TargetPrice = targetPrice
        };
}
