using System;
using System.Text.Json.Serialization;

namespace goHomeAssistant.Service;

public class EntityState<T>
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; }
    [JsonPropertyName("last_changed")]
    public DateTime LastChangedAt { get; set; }
    [JsonPropertyName("state")]
    public string State { get; set; }
    
    [JsonPropertyName("attributes")]
    public T Attributes { get; set; }
}