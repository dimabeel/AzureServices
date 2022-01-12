using Azure;
using Azure.Identity;
using AzureServices.JsonProperties;

namespace AzureServices.Services;

public abstract class AzureService
{
    IConfiguration configuration;

    public AzureService(IConfiguration configuration)
    {
        this.configuration = configuration;
        ServiceProperties = configuration
            .GetSection(nameof(AzureServiceProperties))
            .Get<AzureServiceProperties>();

        var settings = configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();
        ClientCredential = new ClientSecretCredential(settings.TenantId,
            settings.ClientId, settings.ClientSecret);
    }

    protected string GenerateError(RequestFailedException e)
    {
        return $"Exception: {e.ErrorCode} - {e.Status}. Message:{e.Message}";
    }

    protected AzureServiceProperties ServiceProperties { get; set; }

    protected ClientSecretCredential ClientCredential { get; set; }
}