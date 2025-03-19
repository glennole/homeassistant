namespace HomeAssistant.Service.Configuration;

public class Auth0Options
{
    public const string Auth0 = "Auth0";
    public string Audience { get; set; }
    public string Domain { get; set; }
}