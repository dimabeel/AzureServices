using AzureServices.Services.KeyVault.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServices.Pages.Azure.FileStorage;

[Authorize]
public class SecretModel : PageModel
{
    private readonly ILogger<SecretModel> _logger;
    private readonly ISecretService _secretsService;

    public SecretModel(ILogger<SecretModel> logger, ISecretService secretsService)
    {
        _logger = logger;
        _secretsService = secretsService;
    }

    public string Message { get; set; } = string.Empty;

    public async Task OnGet(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _secretsService.GetSecretValueAsync(name);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public async Task OnPostDeleteSecret(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _secretsService.DeleteSecretAsync(name);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public async Task OnPostRestoreDeletedSecret(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Message = await _secretsService.RestoreDeletedSecret(name);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public async Task OnPostSetSecret(string name, string value)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
        {
            Message = await _secretsService.SetSecret(name, value);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public async Task OnPostBackupSecret(string name, string path)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(path))
        {
            Message = await _secretsService.BackupSecret(name, path);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public async Task OnPostRestoreBackupSecret(IFormFile loadFile)
    {
        if (loadFile != null)
        {
            Message = await _secretsService.RestoreBackupSecret(loadFile);
        }
        else
        {
            Message = EmptySecretName;
        }
    }

    public IActionResult OnPostReset()
    {
        return RedirectToPage();
    }

    public void OnPostSecretsProperties()
    {
        Message = _secretsService.LoadSecretsProperties();
    }

    private static readonly string EmptySecretName = "Empty secret name.";
}