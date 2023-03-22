using System.Data.SqlClient;

namespace ProductsService.Data.Entities;

public static class DeserializationExtensions
{
    public static async Task<Product?> DeserializeProductAsync(this SqlDataReader reader,
                                                               CancellationToken cancellationToken = default)
    {
        if (!reader.HasRows)
            return null;

        var idOrdinal = reader.GetOrdinal(nameof(Product.Id));
        var nameOrdinal = reader.GetOrdinal(nameof(Product.Name));
        var descriptionOrdinal = reader.GetOrdinal(nameof(Product.Description));
        var tagsOrdinal = reader.GetOrdinal("Tags");
        var priceOrdinal = reader.GetOrdinal(nameof(Product.Price));

        await reader.ReadAsync(cancellationToken);
        
        var id = reader.GetGuid(idOrdinal);
        var name = reader.GetString(nameOrdinal);
        var description = reader.GetString(descriptionOrdinal);
        var categories = reader.GetString(tagsOrdinal)
                               .Split(',');
        var price = Convert.ToDouble(reader.GetDecimal(priceOrdinal));

        return new (id, name, description, categories, price);
    }

    public static async Task<List<OutboxItem>> DeserializeOutboxItemsAsync(this SqlDataReader reader,
                                                                           CancellationToken cancellationToken = default)
    {
        var outboxItems = new List<OutboxItem>();
        if (!reader.HasRows)
            return outboxItems;

        var idOrdinal = reader.GetOrdinal(nameof(OutboxItem.Id));
        var typeOrdinal = reader.GetOrdinal(nameof(OutboxItem.Type));
        var dataOrdinal = reader.GetOrdinal(nameof(OutboxItem.Data));
        var createdAtUtcOrdinal = reader.GetOrdinal(nameof(OutboxItem.CreatedAtUtc));

        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt64(idOrdinal);
            var type = reader.GetString(typeOrdinal);
            var data = reader.GetString(dataOrdinal);
            var createdAtUtc = reader.GetDateTime(createdAtUtcOrdinal);
            var outboxItem = new OutboxItem(id, type, data, createdAtUtc);
            outboxItems.Add(outboxItem);
        }

        return outboxItems;
    }
}
