namespace ProductsService.Models;

public class ProductCreateModel {
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Tags {get;set;}
    public double Price { get; set; }
}

public class ProductListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}

public class ProductDetailsModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Tags { get; set; }
    public double Price { get; set; }
}
