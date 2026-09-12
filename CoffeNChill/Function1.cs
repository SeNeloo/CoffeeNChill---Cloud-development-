using System.Net;
using System.Text.Json;
using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure.Storage.Blobs;

namespace CoffeeNChill;

public class Function1
{
    private readonly MenuTableService _menuTableService;
    private readonly StaffFileService _staffFileService;

    public Function1(MenuTableService menuTableService, StaffFileService staffFileService)
    {
        _menuTableService = menuTableService;
        _staffFileService = staffFileService;
    }

    [Function("CreateMenuItem")]
    public async Task<HttpResponseData> CreateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
    {
        var menuItem = await JsonSerializer.DeserializeAsync<MenuItem>(req.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

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
        var menuItem = await JsonSerializer.DeserializeAsync<MenuItem>(req.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

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
    [Function("GetStaffDocuments")]
    public async Task<HttpResponseData> GetStaffDocuments(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "staff/documents")] HttpRequestData req)
    {
        var documents = await _staffFileService.GetAllDocumentsAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(documents);

        return response;
    }
    [Function("UploadStaffDocument")]
    public async Task<HttpResponseData> UploadStaffDocument(
   [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "staff/documents/upload")] HttpRequestData req)
    {
        var fileName = req.Url.Query
            .TrimStart('?')
            .Split('&')
            .Select(x => x.Split('='))
            .FirstOrDefault(x => x.Length == 2 && x[0] == "fileName")?[1];

        if (string.IsNullOrWhiteSpace(fileName))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("fileName is required.");
            return badResponse;
        }

        fileName = Uri.UnescapeDataString(fileName);

        using var memoryStream = new MemoryStream();
        await req.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var contentType = req.Headers.TryGetValues("Content-Type", out var values)
            ? values.FirstOrDefault() ?? "application/octet-stream"
            : "application/octet-stream";

        var document = await _staffFileService.UploadAsync(
            memoryStream,
            fileName,
            contentType,
            memoryStream.Length);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(document);

        return response;
    }
    [Function("DownloadStaffDocument")]
    public async Task<HttpResponseData> DownloadStaffDocument(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "staff/documents/download/{fileName}")] HttpRequestData req,
    string fileName)
    {
        try
        {
            var stream = await _staffFileService.DownloadAsync(fileName);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/octet-stream");

            await stream.CopyToAsync(response.Body);

            return response;
        }
        catch (Exception)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Document not found.");

            return notFoundResponse;
        }
    }


}


