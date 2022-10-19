using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HomeAssistant.Service;
using Xunit;

namespace HomeAssistant.Tests;

public class HomeAssistantProxy_Tests
{
    private readonly IHomeAssistantProxy _homeAssistantProxy;

    public HomeAssistantProxy_Tests()
    {
        _homeAssistantProxy = new HomeAssistantProxyMocked();
    }
    
    [Fact]
    public void GetStatesFromHomeAssistant_ReturnsListOfEntityStates()
    {
        IEnumerable<EntityState<Attributes>> states = _homeAssistantProxy.GetEntityStates<Attributes>().Result;
        
        Assert.True(states.Any());
    }

    [Fact]
    public void GetHeavyDutySwitchFromHomeAssistantAPI_ReturnsHeavyDutySwitchAsAnEntityState()
    {
        EntityState<Attributes> heavyDutySwitch = _homeAssistantProxy.GetEntityStateByEntityId<Attributes>("switch.heavy_duty_switch").Result;
        Assert.True(heavyDutySwitch.State == "on" || heavyDutySwitch.State == "off");
    }

    [Fact]
    public void TurnOnHeavyDutySwitch_SwitchIsRegisteredAsOn()
    {
        Switch heavyDutySwitch = new Switch("switch.heavy_duty_switch", _homeAssistantProxy, 6);
        Assert.True(heavyDutySwitch.TurnOn() == State.On);
    }
    
    [Fact]
    public void TurnOffHeavyDutySwitch_SwitchIsRegisteredAsOff()
    {
        Switch heavyDutySwitch = new Switch("switch.heavy_duty_switch", _homeAssistantProxy, 6);
        Assert.True(heavyDutySwitch.TurnOff() == State.Off);
    }
}