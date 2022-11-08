using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Serilog;

namespace HomeAssistant.Service;

public interface IHomeAssistantProxy
{
    Task<IEnumerable<EntityState<T>>> TurnOnSwitch<T>(string entityId);
    Task<IEnumerable<EntityState<T>>> TurnOffSwitch<T>(string entityId);

    Task<IEnumerable<EntityState<T>>> GetEntityStates<T>();
    Task<EntityState<T>> GetEntityStateByEntityId<T>(string entityId);
}

public class HomeAssistantProxy : IHomeAssistantProxy
{
    private readonly HttpClient _client;
    private readonly HomeAssistantOptions _options;
    
    public HomeAssistantProxy(IOptions<HomeAssistantOptions> options)
    {
        _options = options.Value;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
        _client.BaseAddress = new Uri(_options.BaseURI);
    }
    
    public async Task<IEnumerable<EntityState<T>>> GetEntityStates<T>()
    {
        Log.Debug("Requesting all entities states");
        HttpResponseMessage response = await _client.GetAsync("states");
        response.EnsureSuccessStatusCode();
        string responseJson = await response.Content.ReadAsStringAsync();
        Log.Debug("Response from get all entities states request: {@entityStateResponses}", responseJson);

        return JsonSerializer.Deserialize<IEnumerable<EntityState<T>>>(responseJson);
    }

    public async Task<EntityState<T>> GetEntityStateByEntityId<T>(string entityId)
    {
        Log.Debug("Requesting entity states from {@entityId}", entityId);
        HttpResponseMessage response = await _client.GetAsync($"states/{entityId}");
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        Log.Debug("Response from get entity states request for {@entityId}: {entityStateResponse}", entityId, responseJson);
        
        return JsonSerializer.Deserialize<EntityState<T>>(responseJson);
    }

    public async Task<IEnumerable<EntityState<T>>> TurnOnSwitch<T>(string entityId)
    {
        Log.Debug("Requesting switch to turn on for {@entityId}", entityId);
        var data = new
        {
            entity_id = entityId
        };
        var content = JsonContent.Create(data);
        HttpResponseMessage response = await _client.PostAsync($"services/switch/turn_on",content);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        Log.Debug("Response from turn on switch request for {@entityId}: {@entityStateResponse}", entityId, responseJson);
        
        return JsonSerializer.Deserialize<IEnumerable<EntityState<T>>>(responseJson);
    }
    
    public async Task<IEnumerable<EntityState<T>>> TurnOffSwitch<T>(string entityId)
    {
        Log.Debug("Requesting switch to turn off for {@entityId}", entityId);
        var data = new
        {
            entity_id = entityId
        };
        var content = JsonContent.Create(data);
        HttpResponseMessage response = await _client.PostAsync($"services/switch/turn_off",content);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        Log.Debug("Response from turn off switch for {@entityId}: {@entityStateResponse}", entityId, responseJson);

        return JsonSerializer.Deserialize<IEnumerable<EntityState<T>>>(responseJson);
    }
}