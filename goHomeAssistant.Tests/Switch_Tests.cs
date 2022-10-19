using goHomeAssistant.Service;
using Xunit;

namespace goHomeAssistant.Tests;

public class SwitchTest
{
    private readonly Switch _switch;
    private readonly Sensor<NordPoolAttributes> _sensor;
    
    public SwitchTest()
    {
        IHomeAssistantProxy homeAssistantProxy = new HomeAssistantProxyMocked();
        _switch = new Switch("switch.heavy_duty_switch", homeAssistantProxy, 6);
    }

    [Fact]
    public void TurnOnSwitch_ReturnsStateOn()
    {
        Assert.Equal(State.On, _switch.TurnOn());
    }

    [Fact]
    public void TurnOffSwicth_ReturnsStateOff()
    {
        Assert.Equal(State.Off, _switch.TurnOff());
    }

    [Fact]
    public void StateHasChangedAfterSwitchIsTurnedOnWhenInitiallyTurnedOff()
    {
        _switch.TurnOff();
        var changedStateAtBefore = _switch.StateChangedAt;

        _switch.TurnOn();
        Assert.Equal(State.On, _switch.State);
        Assert.True(_switch.StateChangedAt > changedStateAtBefore);
    }
    
    [Fact]
    public void StateHasNotChangedAfterSwitchIsTurnedOnWhenInitiallyTurnedOn()
    {
        _switch.TurnOn();
        var stateChangedAtBefore = _switch.StateChangedAt;

        
        _switch.TurnOn();
        Assert.Equal(State.On, _switch.State);
        Assert.Equal(stateChangedAtBefore, _switch.StateChangedAt);
    }
    
    [Fact]
    public void StateHasChangedAfterSwitchIsTurnedOffWhenInitiallyTurnedOn()
    {
        _switch.TurnOn();
        var stateChangedAtBefore = _switch.StateChangedAt;

        _switch.TurnOff();
        Assert.Equal(State.Off, _switch.State);
        Assert.True(_switch.StateChangedAt > stateChangedAtBefore);
    }
    
    [Fact]
    public void StateHasNotChangedAfterSwitchIsTurnedOffWhenInitiallyTurnedOff()
    {
        _switch.TurnOff();
        var stateChangedAtBefore = _switch.StateChangedAt;

        _switch.TurnOff();
        Assert.Equal(State.Off, _switch.State);
        Assert.Equal(stateChangedAtBefore, _switch.StateChangedAt);
    }

    
}