namespace AzureServices.Services.KeyVault;

public abstract class KeyVaultService : AzureService
{
    protected readonly string BackupExtension = "secretbackup";

    protected KeyVaultService(IConfiguration configuration) : base(configuration)
    {
    }
}