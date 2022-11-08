using System.Text.Json.Serialization;

namespace HomeAssistant.Service;

public class NordpoolSensor : Sensor<NordPoolAttributes>
{
    public NordpoolSensor(string entityId, IHomeAssistantProxy homeAssistantProxy) : base(entityId, homeAssistantProxy)
    { }

    public int[] GetPeakHours()
    {
        var peakHours = new List<int>();
        for (var i = 0; i < 24; i++)
        {
            switch (i)
            {
                case 0:
                    if(Attributes.Today[0] > Attributes.Today[1])
                        peakHours.Add(0);
                    break;
                case 23:
                    if(Attributes.Today[23] > Attributes.Today[22])
                        peakHours.Add(23);
                    break;
                case < 23 and > 0:
                    if(Attributes.Today[i] > Attributes.Today[i-1] && Attributes.Today[i] > Attributes.Today[i+1])
                        peakHours.Add(i);
                    break;
            }
        }

        return peakHours.ToArray();
    }

    public IEnumerable<int[]> GetTodaysPeriodesAboveAveragePrice()
    {
        var averagePrice = Attributes.Average;
        var peakHours = GetPeakHours();

        var periods = new List<int[]>();

        foreach (var peakHour in peakHours)
        {
            var periodHours = new List<int>();
            if(Attributes.Today[peakHour] < averagePrice)
                continue;

            periodHours.Add(peakHour);
            for (int i = peakHour - 1; i >= 0; i--)
            {
                if(Attributes.Today[i] <= Attributes.Today[i + 1] && Attributes.Today[i] >= averagePrice)
                    periodHours.Add(i);
                else
                    break;
            }
                
            for (int i = peakHour + 1; i <= 23; i++)
            {
                if(Attributes.Today[i] <= Attributes.Today[i - 1] && Attributes.Today[i] >= averagePrice)
                    periodHours.Add(i);
                else
                    break;
            }

            periods.Add(periodHours.OrderBy(d => d).ToArray());
        }

        return periods;
    }
    
    public IEnumerable<int[]> GetTodaysPeriodesBelowAveragePrice()
    {
        var averagePrice = Attributes.Average;
        var bottomHours = GetBottomHours();

        var periods = new List<int[]>();

        foreach (var bottomHour in bottomHours)
        {
            var periodHours = new List<int>();
            if(Attributes.Today[bottomHour] > averagePrice)
                continue;

            periodHours.Add(bottomHour);
            for (int i = bottomHour - 1; i >= 0; i--)
            {
                if(Attributes.Today[i] >= Attributes.Today[i + 1] && Attributes.Today[i] <= averagePrice)
                    periodHours.Add(i);
                else
                    break;
            }
                
            for (int i = bottomHour + 1; i <= 23; i++)
            {
                if(Attributes.Today[i] >= Attributes.Today[i - 1] && Attributes.Today[i] <= averagePrice)
                    periodHours.Add(i);
                else
                    break;
            }

            periods.Add(periodHours.OrderBy(d => d).ToArray());
        }

        return periods;
    }
    
    public IEnumerable<int> GetBottomHours()
    {
        var bottomHours = new List<int>();
        for (var i = 0; i < 24; i++)
        {
            switch (i)
            {
                case 0:
                    if(Attributes.Today[0] < Attributes.Today[1])
                        bottomHours.Add(0);
                    break;
                case 23:
                    if(Attributes.Today[23] < Attributes.Today[22])
                        bottomHours.Add(23);
                    break;
                case < 23 and > 0:
                    if(Attributes.Today[i] < Attributes.Today[i-1] && Attributes.Today[i] < Attributes.Today[i+1])
                        bottomHours.Add(i);
                    break;
            }
        }
        return bottomHours;
    }

}

public class NordPoolAttributes : Attributes
{
    [JsonPropertyName("current_price")]
    public double CurrentPrice { get; set; }

    [JsonPropertyName("average")]
    public double Average { get; set; }
    
    [JsonPropertyName("min")]
    public double Min { get; set; }
    
    [JsonPropertyName("max")]
    public double Max { get; set; }

    [JsonPropertyName("today")]
    public double[] Today { get; set; }
    
    [JsonPropertyName("tomorrow")]
    public double[] Tomorrow { get; set; }
    
}