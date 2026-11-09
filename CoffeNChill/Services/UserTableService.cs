using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services;

public class UserTableService
{
    private readonly TableClient _tableClient;

    public UserTableService(string connectionString)
    {
        var serviceClient = new TableServiceClient(connectionString);

        _tableClient = serviceClient.GetTableClient("Users");

        _tableClient.CreateIfNotExists();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await foreach (User user in _tableClient.QueryAsync<User>(
            user => user.Email == email))
        {
            return user;
        }

        return null;
    }

    public async Task CreateUserAsync(User user)
    {
        await _tableClient.AddEntityAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<User>(
                "Users",
                userId);

            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}