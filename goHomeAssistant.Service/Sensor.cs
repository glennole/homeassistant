namespace goHomeAssistant.Service;

public class Sensor<T>
{
    private readonly string _entityId;
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    
    public T Attributes { get; private set; }
    
    public Sensor(string entityId, IHomeAssistantProxy homeAssistantProxy)
    {
        _entityId = entityId;
        _homeAssistantProxy = homeAssistantProxy;

        Attributes = GetReadings();
    }

    public T GetReadings()
    {
        var result = _homeAssistantProxy.GetEntityStateByEntityId<T>(_entityId).Result.Attributes;

        return result;
    }
}