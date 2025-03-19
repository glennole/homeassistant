namespace HomeAssistant.Service.Configuration;

public class HomeAssistantServiceConfiguration
{
    public HomeAssistantOptions HomeAssistant { get; set; }
    public JobsOptions Jobs { get; set; }
    public PostgresqlOptions Postgresql { get; set; }
    public SendGridOptions SendGrid { get; set; }
    public Auth0Options Auth0 { get; set; }
}