using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrderMonitorClient;
using MudBlazor.Services;
using OrderMonitorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress =
    new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<OrderMonitorService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
