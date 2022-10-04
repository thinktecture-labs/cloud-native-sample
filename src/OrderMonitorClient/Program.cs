using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrderMonitorClient;
using MudBlazor.Services;
using OrderMonitorClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("OrderMonitor")
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { builder.Configuration.GetValue<string>("ApiRoot") });
        return handler;
    });

builder.Services.AddScoped(services => services.GetRequiredService<IHttpClientFactory>()
    .CreateClient("OrderMonitor"));

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
});

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress =
//    new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<OrderMonitorService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
