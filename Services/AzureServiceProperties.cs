namespace AzureServices.Services;

sealed public class AzureServiceProperties
{
    public string KeyVaultUri { get; set; } = string.Empty;

    public string QueueName { get; set; } = string.Empty;

    public string StorageConnectionString { get; set; } = string.Empty;
}