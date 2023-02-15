namespace HomeAssistant.Service.Vault;

public class VaultOptions
{
    public string Address { get; set; }
    public string Token { get; set; }
    public string MountPath { get; set; }
    public string Secret { get; set; }
}
