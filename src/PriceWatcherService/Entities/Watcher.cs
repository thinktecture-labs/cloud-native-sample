namespace PriceWatcher.Entities;

public class Watcher
{
    public string Email { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public double Price { get; set; }
}
