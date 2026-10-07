using Azure;
using Azure.Data.Tables;
using System.Text.Json;

namespace CoffeeNChill.Services;

public class OrderTableService
{
    private readonly TableClient _tableClient;

    public OrderTableService(string connectionString)
    {
        var serviceClient = new TableServiceClient(connectionString);

        _tableClient = serviceClient.GetTableClient("Orders");

        _tableClient.CreateIfNotExists();
    }

    public async Task CreateOrderAsync(
        string orderId,
        string customerName,
        List<string> selectedItemSKUs,
        double totalPrice,
        DateTime orderTimestamp,
        string orderDate,
        string status)
    {
        var entity = new TableEntity
        {
            PartitionKey = orderDate,
            RowKey = orderId
        };

        entity["CustomerName"] = customerName;
        entity["SelectedItemSKUs"] =
            JsonSerializer.Serialize(selectedItemSKUs);
        entity["TotalPrice"] = totalPrice;
        entity["OrderTimestamp"] = orderTimestamp;
        entity["Status"] = status;

        await _tableClient.AddEntityAsync(entity);
    }

    public async Task UpdateOrderStatusAsync(
        string orderDate,
        string orderId,
        string status)
    {
        var response = await _tableClient.GetEntityAsync<TableEntity>(
            orderDate,
            orderId);

        var entity = response.Value;

        entity["Status"] = status;

        await _tableClient.UpdateEntityAsync(
            entity,
            entity.ETag,
            TableUpdateMode.Merge);
    }
}
