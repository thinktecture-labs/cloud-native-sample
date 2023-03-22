namespace ProductsService.Data.Entities;

public sealed record OutboxItem(long Id, string Type, string Data, DateTime CreatedAtUtc);

public static class OutboxItemTypes
{
    public const string PriceDrop = "PriceDrop";
}
