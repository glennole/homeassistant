using System.Text.Json.Serialization;

namespace goHomeAssistant.Service;

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
    public IEnumerable<double> Today { get; set; }
    
    [JsonPropertyName("tomorrow")]
    public IEnumerable<double> Tomorrow { get; set; }
    
}