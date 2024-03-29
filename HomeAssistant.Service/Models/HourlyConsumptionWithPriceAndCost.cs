namespace HomeAssistant.Service.Models;

public struct HourlyConsumptionWithPriceAndCost
{
    public int Hour;
    public decimal Consumption, Price, Cost;

    public HourlyConsumptionWithPriceAndCost(int hour, decimal consumption, decimal price)
    {
        Hour = hour;
        Consumption = consumption;
        Price = price;
        Cost = consumption * price;
    }
}