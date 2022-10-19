using System.Collections;

namespace HomeAssistant.Service;

public class Switch
{
    private State _state;
    private readonly string _entityId;
    private readonly IHomeAssistantProxy _homeAssistantProxy;

    
    public DateTime StateChangedAt { get; set; }

    public int MinimumOperatingHoursPerDay { get; }
    
    public Switch(string entityId, IHomeAssistantProxy homeAssistantProxy, int minimumOperatingHoursPerDay)
    {
        _entityId = entityId;
        _homeAssistantProxy = homeAssistantProxy;
        MinimumOperatingHoursPerDay = minimumOperatingHoursPerDay;
    }

    public State State
    {
        get => _state;
        private set
        {
            if(_state == value)
                return;
            
            _state = value;
            StateChangedAt = DateTime.Now;
        }
    }

    public State TurnOn()
    {
        List<EntityState<Attributes>> entityStates = _homeAssistantProxy.TurnOnSwitch<Attributes>(_entityId).Result.ToList();

        var entityState = entityStates.FirstOrDefault(es => es.EntityId == _entityId);
        if (entityState == null)
            return State.Unknown;
        State = MapEntityStateToStateEnum(entityState.State);
        return State;
    }

    public State TurnOff() 
    {
        List<EntityState<Attributes>> entityStates = _homeAssistantProxy.TurnOffSwitch<Attributes>(_entityId).Result.ToList();

        var entityState = entityStates.FirstOrDefault(es => es.EntityId == _entityId);
        if (entityState == null)
            return State.Unknown;
        
        State = MapEntityStateToStateEnum(entityState.State);
        return State;
    }

    public static State MapEntityStateToStateEnum(string entityState)
    {
        return entityState.ToLower() switch
        {
            "on" => State.On,
            "off" => State.Off,
            _ => State.Unknown
        };
    }
}