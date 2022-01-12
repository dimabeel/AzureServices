namespace AzureServices.Services.Storage.Blobs;

public interface IBlobService
{
    string GetBlobsNames(string blobName);
    Task<string> DownloadBlobAsync(string blobName, string fileName, string savePath);
    Task<string> UploadBlobAsync(string blobName, string fileName, string path);
    Task<string> ReadBlob(string blobName, string fileName);
}