using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using goHomeAssistant.Service;

namespace goHomeAssistant.Tests;

public class HomeAssistantProxyMocked : IHomeAssistantProxy
{
    public string Token { get; set; }
    public string BaseUrl { get; set; }

    public async Task<IEnumerable<EntityState<T>>> TurnOnSwitch<T>(string entityId)
    {
        var entityStates = new List<EntityState<T>>();
        entityStates.Add(new EntityState<T>() {EntityId = entityId, State = "on", LastChangedAt = DateTime.Now});
        return entityStates;
    }

    public async Task<IEnumerable<EntityState<T>>> TurnOffSwitch<T>(string entityId)
    {
        var entityStates = new List<EntityState<T>>();
        entityStates.Add(new EntityState<T>() {EntityId = entityId, State = "off", LastChangedAt = DateTime.Now});
        return entityStates;
    }

    public Task<IEnumerable<EntityState<T>>> GetEntityStates<T>()
    {
        var entityStates = new List<EntityState<T>>();
        entityStates.Add(new EntityState<T>()
        {
            State = "on",
            EntityId = "switch.heavy_duty_switch",
            LastChangedAt = DateTime.Now
        });
        entityStates.Add(new EntityState<T>()
            {State = "3.456", EntityId = "sensor.nordpool_kwh_krsand_nok_3_095_025", LastChangedAt = DateTime.Now});
        return Task.FromResult<IEnumerable<EntityState<T>>>(entityStates);
    }

    public Task<EntityState<T>> GetEntityStateByEntityId<T>(string entityId)
    {
        var sensor = new EntityState<NordPoolAttributes>()
        {
            State = "3.456",
            EntityId = "sensor.nordpool_kwh_krsand_nok_3_095_025",
            LastChangedAt = DateTime.Now,
            Attributes = new NordPoolAttributes()
            {
                CurrentPrice = 5.89,
                Today = new List<double>()
                {
                    1.00,2.00,1.12,1.15,1.26,1.56,5.56,5.89,1.00,2.00,1.12,1.15,1.26,1.56,5.56,5.89,1.00,2.00,1.12,1.15,1.26,1.56,5.56,5.89
                },
                Max = 5.89
            }
        };

        var heavySwitch = new EntityState<Attributes>()
        {
            State = "on", 
            EntityId = "switch.heavy_duty_switch", 
            LastChangedAt = DateTime.Now
        };

        var jsonString = JsonSerializer.Serialize(sensor);
        jsonString += ", " +(JsonSerializer.Serialize(heavySwitch));
        jsonString = "[" + jsonString + "]";
        
        var result = JsonSerializer.Deserialize<IEnumerable<EntityState<T>>>(jsonString);
        return Task.FromResult(result.FirstOrDefault(entity => entity.EntityId == entityId));
        
    }
}