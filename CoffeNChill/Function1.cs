using System.Net;
using System.Text.Json;
using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CoffeeNChill;

public class Function1
{
    private readonly MenuTableService _menuTableService;

    public Function1(MenuTableService menuTableService)
    {
        _menuTableService = menuTableService;
    }

    [Function("CreateMenuItem")]
    public async Task<HttpResponseData> CreateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
    {
        var menuItem = await JsonSerializer.DeserializeAsync<MenuItem>(req.Body);

        if (menuItem == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid menu item.");
            return badResponse;
        }

        if (string.IsNullOrWhiteSpace(menuItem.PartitionKey))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Category is required.");
            return badResponse;
        }

        if (string.IsNullOrWhiteSpace(menuItem.RowKey))
        {
            menuItem.RowKey = Guid.NewGuid().ToString();
        }

        await _menuTableService.CreateMenuItemAsync(menuItem);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(menuItem);
        return response;
    }

    [Function("GetAllMenuItems")]
    public async Task<HttpResponseData> GetAllMenuItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req)
    {
        var menuItems = await _menuTableService.GetAllMenuItemsAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(menuItems);
        return response;
    }

    [Function("GetMenuItemsByCategory")]
    public async Task<HttpResponseData> GetMenuItemsByCategory(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")]
        HttpRequestData req,
        string category)
    {
        var menuItems = await _menuTableService.GetMenuItemsByCategoryAsync(category);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(menuItems);
        return response;
    }

    [Function("UpdateMenuItem")]
    public async Task<HttpResponseData> UpdateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")]
        HttpRequestData req,
        string category,
        string id)
    {
        var menuItem = await JsonSerializer.DeserializeAsync<MenuItem>(req.Body);

        if (menuItem == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid menu item.");
            return badResponse;
        }

        menuItem.PartitionKey = category;
        menuItem.RowKey = id;

        await _menuTableService.UpdateMenuItemAsync(menuItem);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(menuItem);
        return response;
    }

    [Function("DeleteMenuItem")]
    public async Task<HttpResponseData> DeleteMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")]
        HttpRequestData req,
        string category,
        string id)
    {
        await _menuTableService.DeleteMenuItemAsync(category, id);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
