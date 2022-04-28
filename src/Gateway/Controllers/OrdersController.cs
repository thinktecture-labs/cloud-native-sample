using System.Text.Json;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    
    private readonly int _daprHttpPort;

    public OrdersController(IConfiguration configuration)
    {
        _daprHttpPort = configuration.GetValue<int>("DAPR_HTTP_PORT");
    }
    
    // GET
    [HttpGet]
    [Route("monitor")]
    public async Task<IActionResult> GetOrderMonitorDataAsync()
    {
        var client = new HttpClient();
        var getOrders = client.GetFromJsonAsync<List<JsonElement>>(BuildDaprUrlFor("orders", "orders"));
        var getProducts = client.GetFromJsonAsync<List<JsonElement>>(BuildDaprUrlFor("products", "products"));

        var res = await Task.WhenAll(getOrders, getProducts);
        var orders = res[0];
        var products = res[1];
        
        return Ok(orders.Select(o =>
        {
            var res =  new OrderMonitorListModel
            {
                Id = o.GetProperty("id").GetGuid(),
                UserId = o.GetProperty("userId").GetString() ?? string.Empty,
                Positions = new List<OrderMonitorPositionModel>()
            };
            foreach (dynamic pos in o.GetProperty("positions").EnumerateArray())
            {
                res.Positions.Add(TransformPosition(products, pos));
            }
            return res;
        }));
    }

    private OrderMonitorPositionModel TransformPosition(List<JsonElement> products, JsonElement pos)
    {
        var found = products.FirstOrDefault(pr => pr.GetProperty("id").GetString() == pos.GetProperty("productId").GetString());
        return new OrderMonitorPositionModel
        {
            ProductId = pos.GetProperty("productId").GetGuid(),
            ProductName = found.GetProperty("name").GetString() ?? string.Empty,
            ProductDescription = found.GetProperty("description").GetString() ?? string.Empty,
            ProductPrice = found.GetProperty("price").GetDouble(),
            Quantity = pos.GetProperty("quantity").GetInt32()
        };
    }
    

    private string BuildDaprUrlFor(string service, string path)
    {
        return $"http://localhost:{_daprHttpPort}/v1.0/invoke/{service}/method/{path}";
    }
}
