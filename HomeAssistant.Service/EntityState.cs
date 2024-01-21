using System;
using System.Text.Json.Serialization;

namespace HomeAssistant.Service;

public class EntityState<T>
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; } = string.Empty;
    
    [JsonPropertyName("last_changed")]
    public DateTime LastChangedAt { get; set; }

    [JsonPropertyName("state")] 
    public string State { get; set; } = string.Empty;
    
    [JsonPropertyName("attributes")]
    public T Attributes { get; set; }
}