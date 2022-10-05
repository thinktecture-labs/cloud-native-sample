using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotificationService;
using NotificationService.Configuration;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var cfg = new NotificationServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(NotificationServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException($"Configuration not found. Please specify the {NotificationServiceConfiguration.SectionName} section");
}

builder.Services.AddSingleton(cfg);

// Configure AuthN
var notificationHubEndpoint = "/notifications/notificationHub";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        //todo: check if we can put authority in DI
        options.Authority = builder.Configuration.GetValue<string>("Authority");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments(notificationHubEndpoint)))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Configure AuthZ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "sample");
    });
});

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>(notificationHubEndpoint)
    .RequireAuthorization("ApiScope");

app.MapMetrics();
app.UseHttpMetrics();

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.MapControllers();

app.Run();
