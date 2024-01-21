namespace HomeAssistant.Service.Configuration;

public class PostgresqlOptions
{
    public const string Postgresql = "Postgresql";
    public string ConnectionString { get; set; } = string.Empty;
}