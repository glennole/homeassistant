namespace HomeAssistant.Service.Configuration;

public class AzureKeyVaultOptions
{
    public const string AzureKeyVault = "AzureKeyVault";

    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SecretName { get; set; } = string.Empty;
    public string KeyVaultName { get; set; } = string.Empty;
}