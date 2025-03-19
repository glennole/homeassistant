namespace HomeAssistant.Service;

public class HomeAssistantOptions
{
    public const string HomeAssistant = "HomeAssistant";
    public string Token { get; set; } = string.Empty;
    public string BaseURI { get; set; } = string.Empty;
}