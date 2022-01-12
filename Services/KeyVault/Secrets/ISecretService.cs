namespace AzureServices.Services.KeyVault.Secrets;

public interface ISecretService
{
    Task<string> GetSecretValueAsync(string name);
    Task<string> DeleteSecretAsync(string name);
    Task<string> RestoreDeletedSecret(string name);
    Task<string> SetSecret(string name, string value);
    Task<string> BackupSecret(string name, string path);
    Task<string> RestoreBackupSecret(IFormFile file);
    string LoadSecretsProperties();
}