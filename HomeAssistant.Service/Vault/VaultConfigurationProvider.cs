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
        var homeAssistantToken = (await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync("homeassistant/token", _config.MountPath)).Data["token"].ToString();
        var homeAssistantBaseUri= (await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync("homeassistant/baseuri", _config.MountPath)).Data["baseuri"].ToString();
        
        Data.Add("HomeAssistantOptions:Token", homeAssistantToken);
        Data.Add("HomeAssistantOptions:BaseUri", homeAssistantBaseUri);
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