namespace PriceWatcher.Entities;

public class Watcher 
{
    public string Email { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
}
