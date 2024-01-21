using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using HomeAssistant.Service.Configuration;
using Microsoft.AspNetCore.Http;

namespace HomeAssistant.Service.Vault;

public class AzureKeyVaultProvider : ConfigurationProvider
{
    private AzureKeyVaultOptions _azureKeyVaultOptions;
    
    public AzureKeyVaultProvider(AzureKeyVaultOptions azureKeyVaultOptions)
    {
        _azureKeyVaultOptions = azureKeyVaultOptions;
    }

    public override void Load()
    {
        LoadAsync().Wait();
    }

    public async Task LoadAsync()
    {
        await GetHomeAssistantCredentials();
    }

    public async Task GetHomeAssistantCredentials()
    {
        var kvUri = $"https://{_azureKeyVaultOptions.KeyVaultName}.vault.azure.net";
        var client = new SecretClient(new Uri(kvUri),
            new ClientSecretCredential(_azureKeyVaultOptions.TenantId, _azureKeyVaultOptions.ClientId,
                _azureKeyVaultOptions.ClientSecret));
            
        var result = await client.GetSecretAsync(_azureKeyVaultOptions.SecretName);
        HomeAssistantServiceConfiguration configuration = JsonSerializer.Deserialize<HomeAssistantServiceConfiguration>(result.Value.Value);
        
        Data.Add("HomeAssistant:Token", configuration.HomeAssistant.Token);
        Data.Add("HomeAssistant:BaseUri", configuration.HomeAssistant.BaseURI);
        Data.Add("Postgresql:ConnectionString", configuration.Postgresql.ConnectionString);
        Data.Add("Jobs:WaterHeater:CronExp", configuration.Jobs.WaterHeater.CronExp);
        Data.Add("Jobs:Nordpool:CronExp", configuration.Jobs.Nordpool.CronExp);
        Data.Add("SendGrid:ApiKey", configuration.SendGrid.ApiKey);
        Data.Add("Auth0:Audience", configuration.Auth0.Audience);
        Data.Add("Auth0:Domain", configuration.Auth0.Domain);
    }
}

public class AzureKeyVaultConfigurationSource : IConfigurationSource
{
    private AzureKeyVaultOptions _config;

    public AzureKeyVaultConfigurationSource(Action<AzureKeyVaultOptions> config)
    {
        _config = new AzureKeyVaultOptions();
        config.Invoke(_config);
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AzureKeyVaultProvider(_config);
    }
}

public static class AzureKeyVaultExtensions
{
    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder configuration,
        Action<AzureKeyVaultOptions> options)
    {
        var azureKeyVaultOptions = new AzureKeyVaultConfigurationSource(options);
        configuration.Add(azureKeyVaultOptions);
        return configuration;
    }
}