using Azure.Data.Tables;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services;

public class MenuTableService
{
    private readonly TableClient _tableClient;

    public MenuTableService(string connectionString)
    {
        var serviceClient = new TableServiceClient(connectionString);

        _tableClient = serviceClient.GetTableClient("MenuItems");

        _tableClient.CreateIfNotExists();
    }

    public async Task CreateMenuItemAsync(MenuItem menuItem)
    {
        await _tableClient.AddEntityAsync(menuItem);
    }

    public async Task<List<MenuItem>> GetAllMenuItemsAsync()
    {
        var menuItems = new List<MenuItem>();

        await foreach (MenuItem item in _tableClient.QueryAsync<MenuItem>())
        {
            menuItems.Add(item);
        }

        return menuItems;
    }

    public async Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string category)
    {
        var menuItems = new List<MenuItem>();

        await foreach (MenuItem item in _tableClient.QueryAsync<MenuItem>(
            item => item.PartitionKey == category))
        {
            menuItems.Add(item);
        }

        return menuItems;
    }

    public async Task UpdateMenuItemAsync(MenuItem menuItem)
    {
        var existingItem = await _tableClient.GetEntityAsync<MenuItem>(
            menuItem.PartitionKey,
            menuItem.RowKey);

        menuItem.ETag = existingItem.Value.ETag;

        await _tableClient.UpdateEntityAsync(
            menuItem,
            menuItem.ETag,
            TableUpdateMode.Merge);
    }

    public async Task DeleteMenuItemAsync(string category, string id)
    {
        await _tableClient.DeleteEntityAsync(category, id);
    }
}
