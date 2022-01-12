using System.Text;
using Azure.Storage.Blobs;
using AzureServices.Services.KeyVault.Secrets;

namespace AzureServices.Services.Storage.Blobs;

sealed public class BlobService : AzureService, IBlobService
{
    BlobServiceClient _blobServiceClient;

    public BlobService(IConfiguration configuration, ISecretService secretsService) : base(configuration)
    {
        var connectionString = secretsService.GetSecretValueAsync(ServiceProperties.StorageConnectionString).Result;
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    private BlobContainerClient GetBlobContainerClient(string blobName)
    {
        return _blobServiceClient.GetBlobContainerClient(blobName);
    }

    public async Task<string> UploadBlobAsync(string blobName, string fileName, string path)
    {
        BlobClient client = GetBlobContainerClient(blobName).GetBlobClient(fileName);
        if (File.Exists(path))
        {
            await client.UploadAsync(path);
            return "Uploaded";
        }
        else
        {
            return "File not exists";
        }
    }

    public string GetBlobsNames(string blobName)
    {
        var blobs = GetBlobContainerClient(blobName).GetBlobs();
        if (blobs.Count() > 0)
        {
            return blobs.Select(x => x.Name).Aggregate((x, y) => { return $"{x}. {y}."; });
        }
        else
        {
            return "No blobs";
        }
    }

    public async Task<string> DownloadBlobAsync(string blobName, string fileName, string savePath)
    {
        var client = GetBlobContainerClient(blobName).GetBlobClient(fileName);
        var stream = client.OpenRead();
        byte[] buffer = new byte[stream.Length];
        await stream.ReadAsync(buffer, 0, buffer.Length);
        await File.WriteAllBytesAsync(savePath, buffer);
        return "Downloaded";
    }

    public async Task<string> ReadBlob(string blobName, string fileName)
    {
        var client = GetBlobContainerClient(blobName).GetBlobClient(fileName);
        var stream = client.OpenRead();
        byte[] buffer = new byte[stream.Length];
        await stream.ReadAsync(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }
}