namespace CoffeeNChill.Models;

public class StaffDocument
{
    public string PartitionKey { get; set; } = "StaffDocuments";

    public string RowKey { get; set; } = Guid.NewGuid().ToString();

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
}
