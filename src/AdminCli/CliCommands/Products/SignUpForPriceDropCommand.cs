using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AdminCli.HttpAccess;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Products;

public sealed class SignUpForPriceDropCommand : ICliCommand
{
    public SignUpForPriceDropCommand(IHttpService httpService) => HttpService = httpService;
    
    private IHttpService HttpService { get; }
    
    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Sign up with your email address to get notified when a product's price drops.";
        var emailOption =
            config.Option("-e||--email <Email>",
                          "Your email address. We will write an email to this address once the price drops.",
                          CommandOptionType.SingleValue)
                  .IsRequired();

        var productIdOption =
            config.Option("-p|--product-id <ProductId>",
                          "The ID of the product that you want to watch the price on. Must be a GUID. Use the products list command to obtain the GUID of a product.",
                          CommandOptionType.SingleValue)
                  .IsRequired();

        var priceOption =
            config.Option("-t|--target-price <Price>",
                          "The target price. When the product's price is dropped to a value equal or below the target price, an email will be sent.",
                          CommandOptionType.SingleValue);
        
        config.OnExecuteAsync(async cancellationToken =>
        {
            var email = emailOption.Value();
            if (!ValidateEmail(email))
            {
                Console.WriteLine("You did not provide a valid email address");
                return;
            }

            var productIdText = productIdOption.Value();
            if (!Guid.TryParse(productIdText, out var parsedId))
            {
                Console.WriteLine($"Could not parse \"{productIdText}\" to a GUID.");
                return;
            }

            var priceText = priceOption.Value();
            if (!double.TryParse(priceText, CultureInfo.CurrentCulture, out var parsedTargetPrice))
            {
                Console.WriteLine($"Could not parse \"{priceText}\" to a decimal number.");
                return;
            }

            var dto = new SignUpForPriceDropDto(email, parsedId, parsedTargetPrice);
            await HttpService.PostAsync("/pricewatcher/register", dto, cancellationToken);
            Console.WriteLine("You signed up successfully.");
        });
    }

    private static bool ValidateEmail([NotNullWhen(true)] string? email)
    {
        // We simply check if the email address contain an @ which is not placed at the beginning or end.
        if (email is null || email.Length < 3)
            return false;
        
        var indexOfAt = email.IndexOf('@');
        return indexOfAt > 0 && indexOfAt != email.Length - 1;
    }
}

// The properties of this record are read by the JSON serializer
// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record SignUpForPriceDropDto(string Email, Guid ProductId, double Price);
// ReSharper restore NotAccessedPositionalProperty.Global
