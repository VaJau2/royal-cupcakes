using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

//Настройки управления
//Кнопки в настройках должны называться так же, как их экшены
public partial class ControlsModal: AbstractModal
{
    private readonly string[] actions = {
        "ui_up", "ui_left", "ui_down", "ui_right",
        "ui_run", "ui_sit", "ui_rotate_left", "ui_rotate_right"
    };
    
    private string monitoringAction;
    private string oldButtonName;
    private Button tempActionButton;

    private void OnControlKeyPressed(string action)
    {
        ClearAction();
        monitoringAction = action;
        tempActionButton = GetNode<Button>($"buttons/{action}");
        oldButtonName = tempActionButton.Text;
        tempActionButton.Text = "...";
    }
    
    protected override void CloseModal()
    {
        ClearAction();
        base.CloseModal();
    }

    private void ClearAction()
    {
        if (string.IsNullOrEmpty(monitoringAction)) return;
        monitoringAction = null;
        tempActionButton.Text = oldButtonName;
    }

    private bool KeyIsFree(Key key)
    {
        foreach (var action in actions)
        {
            foreach (var actionEvent in InputMap.ActionGetEvents(action))
            {
                if (actionEvent is not InputEventKey actionKey) continue;
                if (actionKey.PhysicalKeycode == key) return false;
            }
        }

        return true;
    }

    private bool SameActionKey(string action, Key key)
    {
        foreach (var actionEvent in InputMap.ActionGetEvents(action))
        {
            if (actionEvent is not InputEventKey actionKey) continue;
            if (actionKey.PhysicalKeycode == key) return true;
        }

        return false;
    }

    public override void _Ready()
    {
        foreach (var action in actions)
        {
            var settingsKey = Settings.Instance.GetActionKey(action);
            if (settingsKey == "") continue;

            var key = OS.FindKeycodeFromString(settingsKey);
            var keyAction = new InputEventKey();
            keyAction.PhysicalKeycode = key;
            
            var actionButton = GetNode<Button>($"buttons/{action}");
            actionButton.Text = OS.GetKeycodeString(keyAction.PhysicalKeycode);
            InputMap.ActionEraseEvents(action);
            InputMap.ActionAddEvent(action, keyAction);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (string.IsNullOrEmpty(monitoringAction)) return;
        if (@event is not InputEventKey { Pressed: true } eventKey) return;

        if (Input.IsActionJustPressed("ui_cancel") || SameActionKey(monitoringAction, eventKey.PhysicalKeycode))
        {
            ClearAction();
            return;
        }
        
        if (!KeyIsFree(eventKey.PhysicalKeycode)) return;

        InputMap.ActionEraseEvents(monitoringAction);
        InputMap.ActionAddEvent(monitoringAction, eventKey);
        var keyString = OS.GetKeycodeString(eventKey.PhysicalKeycode);
        Settings.Instance.SetActionKey(monitoringAction, keyString);
        tempActionButton.Text = keyString;
        tempActionButton.ButtonPressed = false;
        tempActionButton = null;
        monitoringAction = oldButtonName = null;
    }
}
