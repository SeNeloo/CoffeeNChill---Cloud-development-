using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoffeeNChill.Models;
using System.IO;
using CoffeeNChill.Models;

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
    public async Task<StaffDocument> UploadAsync(
    Stream fileStream,
    string fileName,
    string contentType,
    long fileSize)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            fileStream,
            overwrite: true);

        return new StaffDocument
        {
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            UploadedOn = DateTime.UtcNow
        };
    }
    public async Task<List<StaffDocument>> GetAllDocumentsAsync()
    {
        var documents = new List<StaffDocument>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync())
        {
            documents.Add(new StaffDocument
            {
                FileName = blobItem.Name,
                ContentType = blobItem.Properties.ContentType ?? "application/octet-stream",
                FileSize = blobItem.Properties.ContentLength ?? 0
            });
        }

        return documents;
    }

    public async Task<Stream> DownloadAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadStreamingAsync();

        return response.Value.Content;
    }

}
