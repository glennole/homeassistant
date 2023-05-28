using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace HomeAssistant.Service.Vault;

public class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultOptions _config;
    private readonly IVaultClient _client;

    public VaultConfigurationProvider(VaultOptions config)
    {
        _config = config;
        var tokenAuthMethod = new TokenAuthMethodInfo(_config.Token);

        var vaultClientSettings = new VaultClientSettings(
            _config.Address,
            tokenAuthMethod
        );
        _client = new VaultClient(vaultClientSettings);
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
        var homeAssistantSecret= (await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync(_config.Secret, _config.MountPath));
        
        Data.Add("HomeAssistantOptions:Token", homeAssistantSecret.Data["Token"].ToString());
        Data.Add("HomeAssistantOptions:BaseUri", homeAssistantSecret.Data["BaseURI"].ToString());
        Data.Add("PostgresqlOptions:ConnectionString", homeAssistantSecret.Data["PostgresqlConnectionstring"].ToString());
        Data.Add("Jobs:WaterHeater:CronExp", homeAssistantSecret.Data["Jobs:WaterHeater:CronExp"].ToString());
        Data.Add("Jobs:Nordpool:CronExp", homeAssistantSecret.Data["Jobs:Nordpool:CronExp"].ToString());
        Data.Add("SendGrid:ApiKey", homeAssistantSecret.Data["SendGrid:ApiKey"].ToString());
        Data.Add("Auth0:Audience", homeAssistantSecret.Data["Auth0:Audience"].ToString());
        Data.Add("Auth0:Domain", homeAssistantSecret.Data["Auth0:Domain"].ToString());
    }
}

public class VaultConfigurationSource : IConfigurationSource
{
    private VaultOptions _config;

    public VaultConfigurationSource(Action<VaultOptions> config)
    {
        _config = new VaultOptions();
        config.Invoke(_config);
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(_config);
    }
}

public static class VaultExtensions
{
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder configuration,
        Action<VaultOptions> options)
    {
        var vaultOptions = new VaultConfigurationSource(options);
        configuration.Add(vaultOptions);
        return configuration;
    }
}