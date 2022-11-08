using System.Collections;
using Serilog;

namespace HomeAssistant.Service;

public class Switch
{
    private readonly string _entityId;
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    
    public Switch(string entityId, IHomeAssistantProxy homeAssistantProxy)
    {
        _entityId = entityId;
        _homeAssistantProxy = homeAssistantProxy;
    }

    public DateTime LastChangedAt { get; private set; }
    public State State { get; private set; }

    public double GetUsage()
    {
        throw new NotImplementedException();
    }

    public State GetState()
    {
        Log.Debug("Start getting state of switch({@entityId})...", _entityId);
        EntityState<Attributes> entityState = _homeAssistantProxy.GetEntityStateByEntityId<Attributes>(_entityId).Result;

        if (entityState == null)
        {
            Log.Warning("No state received when fetching state from switch({@entityId}", _entityId);
            return State.Unknown;
        }

        State = MapEntityStateToStateEnum(entityState.State);
        LastChangedAt = entityState.LastChangedAt;
        Log.Debug("Switch({@entityId}) is turned {@state}.", _entityId, State);
       
        return State;
    }

    public State TurnOn()
    {
        Log.Debug("Start turning on switch({@entityId})...", _entityId);
        List<EntityState<Attributes>> entityStates = _homeAssistantProxy.TurnOnSwitch<Attributes>(_entityId).Result.ToList();

        var entityState = entityStates.FirstOrDefault(es => es.EntityId == _entityId);
        if (entityState == null)
        {
            Log.Warning("No state received when turning on switch({@entityId}", _entityId);
            return State.Unknown;
        }
        
        State = MapEntityStateToStateEnum(entityState.State);
        LastChangedAt = entityState.LastChangedAt;
        if(State == State.On)
            Log.Debug("Switch({@entityId}) was successfully turned on", _entityId);
        else
            Log.Debug("Switch({@entityId}) was not successfully turned on. Current state is {@state}", _entityId, State.ToString());
        
        return State;
    }

    public State TurnOff() 
    {
        Log.Debug("Start turning off switch({@entityId})...", _entityId);
        List<EntityState<Attributes>> entityStates = _homeAssistantProxy.TurnOffSwitch<Attributes>(_entityId).Result.ToList();

        var entityState = entityStates.FirstOrDefault(es => es.EntityId == _entityId);
        if (entityState == null)
        {
            Log.Warning("No state received when turning off switch({@entityId})", _entityId);
            return State.Unknown;
        }
        State = MapEntityStateToStateEnum(entityState.State);
        LastChangedAt = entityState.LastChangedAt;
        
        if(State == State.Off)
            Log.Debug("Switch({@entityId}) was successfully turned off", _entityId);
        else
            Log.Debug("Switch({@entityId}) was not successfully turned off. Current state is {@state}", _entityId, State.ToString());
        
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