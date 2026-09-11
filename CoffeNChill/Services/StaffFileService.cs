using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CoffeeNChill.Services;

public class StaffFileService
{
    private readonly BlobContainerClient _containerClient;

    public StaffFileService(string connectionString)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);

        _containerClient =
            blobServiceClient.GetBlobContainerClient("staff-documents");
    }

    public async Task InitializeAsync()
    {
        await _containerClient.CreateIfNotExistsAsync();
    }
}
