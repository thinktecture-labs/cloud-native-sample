namespace NotificationService.Models;

public class DispatchedOrder
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
}

public class CloudEvent<T>
{
    public T Data { get; set; }
} 
