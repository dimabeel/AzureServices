using Azure.Security.KeyVault.Keys;
using Azure;

namespace AzureServices.Services.KeyVault.Keys;

sealed public class KeyService : KeyVaultService, IKeyService
{
    KeyClient keyClient;

    public KeyService(IConfiguration configuration) : base(configuration)
    {
        keyClient = new KeyClient(new Uri(ServiceProperties.KeyVaultUri), ClientCredential);
    }

    public async Task<string> GetKeyValueAsync(string keyName)
    {
        try
        {
            var key = await keyClient.GetKeyAsync(keyName);
            return key.Value.Id.Segments.Last().ToString();
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> CreateKeyAsync(KeyType type, string name)
    {
        try
        {
            var expiresDate = DateTime.Now.AddYears(1);
            switch (type)
            {
                case KeyType.RSA:
                    var rsaKeyOptions = new CreateRsaKeyOptions(name);
                    rsaKeyOptions.ExpiresOn = expiresDate;
                    rsaKeyOptions.Tags.Add("RSA", $"Key {expiresDate.ToString("yyyyMMddHHmmss")}");
                    var rsaKey = await keyClient.CreateRsaKeyAsync(rsaKeyOptions);
                    return $"Successfull add {name} RSA key.";

                case KeyType.Ec:
                    var esKeyOptions = new CreateEcKeyOptions(name);
                    esKeyOptions.ExpiresOn = expiresDate;
                    esKeyOptions.Tags.Add("Ec", $"Key {expiresDate.ToString("yyyyMMddHHmmss")}");
                    var ecKey = await keyClient.CreateEcKeyAsync(esKeyOptions);
                    return $"Successfull add {name} Ec key.";

                default:
                    return "Wrong key type.";
            }
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> DeleteKeyAsync(string name)
    {
        try
        {
            var deleteOperation = await keyClient.StartDeleteKeyAsync(name);
            return $"Key {name} was deleted.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> RecoverDeletedKeyAsync(string name)
    {
        try
        {
            var deleteOperation = await keyClient.StartRecoverDeletedKeyAsync(name);
            return $"Key {name} was recovered.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> BackupKeyAsync(string name, string path)
    {
        try
        {
            byte[] secret = await keyClient.BackupKeyAsync(name);
            var file = $"{DateTime.Now.ToString("yyyymmdd")}{name}.{BackupExtension}";
            var pathToSave = Path.Combine(path, file);
            await File.WriteAllBytesAsync(pathToSave, secret);

            return $"Key with name {name} backup success.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> RestoreBackupKeyAsync(IFormFile file)
    {
        try
        {
            byte[] bytes;
            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
            {
                bytes = binaryReader.ReadBytes((int)file.Length);
            }

            var keyProperties = await keyClient.RestoreKeyBackupAsync(bytes);

            return $"Key with name {keyProperties.Value.Name} backup success.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public string LoadKeysProperties()
    {
        try
        {
            var message = string.Empty;
            var properties = keyClient.GetPropertiesOfKeys();
            foreach (var key in properties)
            {
                message += $"Key: {key.Name}. ";
                if (key.ExpiresOn != null)
                {
                    message += $"Expires on: {key.ExpiresOn} ";
                }

                if (key.Tags.Count > 0)
                {
                    message += "Tags: ";
                    int counter = 1;
                    foreach (var tag in key.Tags)
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

public enum KeyType
{
    Undefined = 0,
    RSA,
    Ec
}