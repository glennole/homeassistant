using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Service.HvaKosterStrommen;

public class HvaKosterStrommenHourPrice
{
    public decimal NOK_per_kWh { get; set; }
    public DateTime time_start { get; set; }
    public DateTime time_end { get; set; }
}