using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrderMonitorClient;
using MudBlazor.Services;
using OrderMonitorClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("OrderMonitor", c =>
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(services => services.GetRequiredService<IHttpClientFactory>()
    .CreateClient("OrderMonitor"));

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress =
    new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<OrderMonitorService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
