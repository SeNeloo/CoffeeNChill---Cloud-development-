using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CoffeeNChill;

public class Function1
{
    private readonly MenuTableService _menuTableService;
    private readonly StaffFileService _staffFileService;
    private readonly OrderTableService _orderTableService;

    public Function1(MenuTableService menuTableService, StaffFileService staffFileService, OrderTableService orderTableService)
    {
        _menuTableService = menuTableService;
        _staffFileService = staffFileService;
        _orderTableService = orderTableService;
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
    [Function("PlaceOrderInQueue")]
    public async Task<HttpResponseData> PlaceOrderInQueue(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/queue")] HttpRequestData req)
    {
        try
        {
            var order = await JsonSerializer.DeserializeAsync<Order>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (order == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid order data.");
                return badResponse;
            }

            if (string.IsNullOrWhiteSpace(order.OrderId))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("OrderId is required.");
                return badResponse;
            }

            if (string.IsNullOrWhiteSpace(order.CustomerName))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("CustomerName is required.");
                return badResponse;
            }

            if (order.SelectedItemSKUs == null || order.SelectedItemSKUs.Count == 0)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("At least one item SKU is required.");
                return badResponse;
            }

            if (order.TotalPrice <= 0)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("TotalPrice must be greater than zero.");
                return badResponse;
            }

            if (order.OrderTimestamp == default)
            {
                order.OrderTimestamp = DateTime.UtcNow;
            }

            if (string.IsNullOrWhiteSpace(order.OrderDate))
            {
                order.OrderDate = order.OrderTimestamp.ToString("yyyy-MM-dd");
            }

            order.Status = "Received";

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var queueClient = new QueueClient(
                connectionString,
                "order-processing-queue");

            await queueClient.CreateIfNotExistsAsync();

            string orderJson = JsonSerializer.Serialize(order);

            await queueClient.SendMessageAsync(orderJson);

            var response = req.CreateResponse(HttpStatusCode.Accepted);

            await response.WriteAsJsonAsync(new
            {
                message = "Order successfully placed in processing queue.",
                orderId = order.OrderId,
                status = order.Status
            });

            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

            await errorResponse.WriteStringAsync(
                $"Failed to place order in queue: {ex.Message}");

            return errorResponse;
        }
    }
    
[Function("ProcessOrderQueue")]
    public async Task ProcessOrderQueue(
    [QueueTrigger("order-processing-queue",
        Connection = "AzureWebJobsStorage")] string queueMessage,
    FunctionContext context)
    {
        var logger = context.GetLogger("ProcessOrderQueue");

        try
        {
            var order = JsonSerializer.Deserialize<Order>(
                queueMessage,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (order == null)
            {
                logger.LogError("Queue message could not be converted into an Order.");
                return;
            }

            logger.LogInformation(
                "Processing order {OrderId} for {CustomerName}.",
                order.OrderId,
                order.CustomerName);

            order.Status = "Received";

            await _orderTableService.CreateOrderAsync(
                order.OrderId,
                order.CustomerName,
                order.SelectedItemSKUs,
                order.TotalPrice,
                order.OrderTimestamp,
                order.OrderDate,
                order.Status);

            logger.LogInformation(
                "Order {OrderId} saved with status Received.",
                order.OrderId);

            order.Status = "Preparing";

            await _orderTableService.UpdateOrderStatusAsync(
                order.OrderDate,
                order.OrderId,
                order.Status);

            logger.LogInformation(
                "Order {OrderId} status updated to Preparing.",
                order.OrderId);

            await Task.Delay(1000);

            order.Status = "Ready";

            await _orderTableService.UpdateOrderStatusAsync(
                order.OrderDate,
                order.OrderId,
                order.Status);

            logger.LogInformation(
                "Order {OrderId} status updated to Ready.",
                order.OrderId);

            await Task.Delay(1000);

            order.Status = "Collected";

            await _orderTableService.UpdateOrderStatusAsync(
                order.OrderDate,
                order.OrderId,
                order.Status);

            logger.LogInformation(
                "Order {OrderId} status updated to Collected.",
                order.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing order queue message.");

            throw;
        }
    }
}



