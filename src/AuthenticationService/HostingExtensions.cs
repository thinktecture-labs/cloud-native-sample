using AuthenticationService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using Duende.IdentityServer;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService.Configuration;
using Microsoft.AspNetCore.HttpOverrides;

namespace AuthenticationService;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // IMPORTANT: change this for non-local dev/test
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.Secure = CookieSecurePolicy.None;
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.AddRazorPages();
        builder.Services.AddHealthChecks();
        
        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                //options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                //options.Events.RaiseSuccessEvents = true;
 
                options.EmitStaticAudienceClaim = true;

                // IMPORTANT: change this for non-local dev/test
                options.Authentication.CookieSameSiteMode = SameSiteMode.Unspecified;
            })
            .AddTestUsers(TestUserProvider.GetAll());

        var cfg = builder.Configuration.GetSection("IdentityServer").Get<IdentityServerConfig>();
        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(IdentityResourcesProvider.GetAll);
        isBuilder.AddInMemoryApiScopes(ApiScopesProvider.GetAll);
        isBuilder.AddInMemoryClients(ClientsProvider.GetAll(cfg.InteractiveClient));
        

        builder.Services.AddAuthentication()
            .AddOpenIdConnect("azuread", "Sign-in with Azure AD", options =>
            {
                options.Authority = cfg.AzureAd.Authority; // "https://login.microsoftonline.com/common";
                options.ClientId = cfg.AzureAd.ClientId; // "b33fe330-1cca-4d71-a286-edabfa622a0b";
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.ResponseType = "id_token";
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidAudience = cfg.AzureAd.ClientId, // "b33fe330-1cca-4d71-a286-edabfa622a0b",

                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            })
            .AddLocalApi(l => l.ExpectedScope = "sample");

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var cfg = app.Configuration.GetSection("IdentityServer").Get<IdentityServerConfig>();
        if (!string.IsNullOrWhiteSpace(cfg.VirtualPath)){
            app.UsePathBase(cfg.VirtualPath);
        }
        app.UseForwardedHeaders();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // IMPORTANT: change this for non-local dev/test
        app.UseCookiePolicy();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        app.MapHealthChecks("/healthz/readiness");
        app.MapHealthChecks("/healthz/liveness");
        return app;
    }
}
