namespace AzureServices.Services.KeyVault.Keys;

public interface IKeyService
{
    Task<string> BackupKeyAsync(string name, string path);
    Task<string> CreateKeyAsync(KeyType type, string name);
    Task<string> DeleteKeyAsync(string name);
    string LoadKeysProperties();
    Task<string> GetKeyValueAsync(string keyName);
    Task<string> RecoverDeletedKeyAsync(string name);
    Task<string> RestoreBackupKeyAsync(IFormFile file);
}