using Azure;
using Azure.Data.Tables;
using System.Text.Json;
using System.Collections.Generic;

namespace CoffeeNChill.Services;

public class OrderTableService
{
    private readonly TableClient _tableClient;

    public OrderTableService(string connectionString)
    {
        Console.WriteLine("OrderTableService: Initializing...");

        var serviceClient = new TableServiceClient(connectionString);
        _tableClient = serviceClient.GetTableClient("Orders");

        Console.WriteLine("OrderTableService: Creating Orders table...");

        _tableClient.CreateIfNotExists();

        Console.WriteLine("OrderTableService: Orders table created or already exists.");
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
        entity["OrderTimestamp"] = new DateTimeOffset(orderTimestamp);
        entity["Status"] = status;

        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
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
    public async Task<List<TableEntity>> GetAllOrdersAsync()
    {
        var orders = new List<TableEntity>();

        await foreach (var entity in _tableClient.QueryAsync<TableEntity>())
        {
            orders.Add(entity);
        }

        return orders;
    }
}
