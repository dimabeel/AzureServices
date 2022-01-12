using AzureServices.Services.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServices.Pages.Azure.Blobs;

[Authorize]
public class BlobModel : PageModel
{
    private readonly ILogger<BlobModel> _logger;
    private readonly IBlobService _blobService;

    public BlobModel(ILogger<BlobModel> logger, IBlobService blobService)
    {
        _logger = logger;
        _blobService = blobService;
    }

    public string Message { get; set; } = string.Empty;

    public async Task OnPostUploadBlob(string blobName, string fileName, string path)
    {
        Message = await _blobService.UploadBlobAsync(blobName, fileName, path);
    }

    public async Task OnPostSaveBlobTo(string blobName, string fileName, string path)
    {
        Message = await _blobService.DownloadBlobAsync(blobName, fileName, path);
    }

    public void OnPostReadBlobs(string blobName)
    {
        var blobs = _blobService.GetBlobsNames(blobName);
        Message = blobs;
    }

    public async Task OnPostReadBlob(string blobName, string fileName)
    {
        var readBlob = await _blobService.ReadBlob(blobName, fileName);
        Message = readBlob;
    }

    public void OnPostReset()
    {
        Message = string.Empty;
    }
}