namespace Gateway.Models;

public class OrderMonitorListModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public IList<OrderMonitorPositionModel> Positions { get; set; }
}

public class OrderMonitorPositionModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public int Quantity { get; set; }
    public double ProductPrice { get; set; }
}
