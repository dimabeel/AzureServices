using AzureServices.Services.KeyVault.Keys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServices.Pages.Azure.FileStorage;

[Authorize]
public class KeyModel : PageModel
{
    private readonly ILogger<KeyModel> _logger;
    private readonly IKeyService _keysService;

    public KeyModel(ILogger<KeyModel> logger, IKeyService keysService)
    {
        _logger = logger;
        _keysService = keysService;
    }

    public string Message { get; set; } = string.Empty;

    public async Task OnGet(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _keysService.GetKeyValueAsync(name);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public async Task OnPostDeleteKey(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _keysService.DeleteKeyAsync(name);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public async Task OnPostRecoverDeletedKey(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _keysService.RecoverDeletedKeyAsync(name);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public async Task OnPostCreateKey(string name, KeyType keyType = KeyType.Undefined)
    {
        if (!string.IsNullOrEmpty(name) && keyType != KeyType.Undefined)
        {
            Message = await _keysService.CreateKeyAsync(keyType, name);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public async Task OnPostBackupKey(string name, string path)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(path))
        {
            Message = await _keysService.BackupKeyAsync(name, path);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public async Task OnPostRecoverBackupKey(IFormFile loadFile)
    {
        if (loadFile != null)
        {
            Message = await _keysService.RestoreBackupKeyAsync(loadFile);
        }
        else
        {
            Message = EmptyKeyName;
        }
    }

    public void OnPostKeysProperties()
    {
        Message = _keysService.LoadKeysProperties();
    }

    private static readonly string EmptyKeyName = "Empty key name.";
}