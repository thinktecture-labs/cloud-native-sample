using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrderMonitorClient;
using MudBlazor.Services;
using OrderMonitorClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;

var builder = WebAssemblyHostBuilder
    .CreateDefault(args);


var http = new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

builder.Services.AddScoped(sp => http);
builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddHttpClient("OrderMonitor",
        client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped(services => services.GetRequiredService<IHttpClientFactory>()
    .CreateClient("OrderMonitor"));


builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
});

builder.Services
    .AddScoped<OrderMonitorService>()
    .AddScoped<ProductsService>();
    
builder.Services.AddMudServices();

await builder.Build().RunAsync();
