using Azure.Security.KeyVault.Secrets;
using Azure;

namespace AzureServices.Services.KeyVault.Secrets;

sealed public class SecretService : KeyVaultService, ISecretService
{
    SecretClient secretClient;

    public SecretService(IConfiguration configuration) : base(configuration)
    {
        secretClient = new SecretClient(new Uri(ServiceProperties.KeyVaultUri), ClientCredential);
    }

    public async Task<string> GetSecretValueAsync(string name)
    {
        try
        {
            var secret = await secretClient.GetSecretAsync(name);
            return secret.Value.Value;
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> DeleteSecretAsync(string name)
    {
        try
        {
            var deleteOperation = await secretClient.StartDeleteSecretAsync(name);
            return $"Secret: {name} was deleted (not permanently).";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> RestoreDeletedSecret(string name)
    {
        try
        {
            var recoverOperation = await secretClient.StartRecoverDeletedSecretAsync(name);
            return $"Secret: {name} was restored.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> SetSecret(string name, string value)
    {
        try
        {
            var secret = await secretClient.SetSecretAsync(name, value);
            return $"Secret with name {name} and value {value} added.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> BackupSecret(string name, string path)
    {
        try
        {
            byte[] secret = await secretClient.BackupSecretAsync(name);
            var file = $"{DateTime.Now.ToString("yyyymmdd")}{name}.{BackupExtension}";
            var pathToSave = Path.Combine(path, file);
            await File.WriteAllBytesAsync(pathToSave, secret);

            return $"Secret with name {name} backup success.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> RestoreBackupSecret(IFormFile file)
    {
        try
        {
            byte[] bytes;
            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
            {
                bytes = binaryReader.ReadBytes((int)file.Length);
            }

            var secretProperties = await secretClient.RestoreSecretBackupAsync(bytes);

            return $"Secret with name {secretProperties.Value.Name} backup success.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public string LoadSecretsProperties()
    {
        try
        {
            var message = string.Empty;
            var secretsProperties = secretClient.GetPropertiesOfSecrets();
            foreach (var secretProperty in secretsProperties)
            {
                message += $"Secret: {secretProperty.Name}. ";

                if (secretProperty.ExpiresOn != null)
                {
                    message += $"Expires on: {secretProperty.ExpiresOn.Value.ToString("yyyy:dd:mm:ss")}. ";
                }

                if (secretProperty.CreatedOn != null)
                {
                    message += $"Created on: {secretProperty.CreatedOn.Value.ToString("yyyy:dd:mm:ss")}. ";
                }

                if (secretProperty.Tags.Count > 0)
                {
                    message += $"Tags: ";
                    int counter = 1;
                    foreach (var tag in secretProperty.Tags)
                    {
                        message += $"{counter}. {tag.Key} - {tag.Value}; ";
                    }
                }
            }

            return message;
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }
}