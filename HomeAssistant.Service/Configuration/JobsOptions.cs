using Microsoft.AspNetCore.Authentication;

namespace HomeAssistant.Service.Configuration;

public class JobsOptions
{
    public const string Jobs = "Jobs";

    public WaterHeater WaterHeater { get; set; }
    public Nordpool Nordpool { get; set; }
    
}

public class WaterHeater
{
    public string CronExp { get; set; } = string.Empty;
}

public class Nordpool
{
    public string CronExp { get; set; } = string.Empty;
}