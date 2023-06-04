using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.Props;
using RoyalCupcakes.System;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters.Player;

/**
 * Класс контроллера висит на игроке и отвечает за управление им
 * управление лассо выделено в отдельный хендлер
 */
public partial class InputController : Node
{
    private const float MinInteractDistance = 2f;
    
    private Character player;
    private Vector3 direction;
    private bool mayMove = true;

    public override void _Ready()
    {
        player = GetParent<Character>();
        CallDeferred(nameof(SetPlayerCharacterDeferred));

        if (!player.IsMultiplayerAuthority())
        {
            SetProcess(false);
            return;
        }

        var pauseMenu = GetNode<PauseMenu>("/root/Main/UI/PauseMenu");
        pauseMenu.Opened += OnPaused;
        
        var camera = GetNode<Camera3D>("../camera");
        camera.Current = true;
        var listener = GetNode<AudioListener3D>("../listener");
        listener.MakeCurrent();
        CallDeferred(nameof(DisableBlackScreenDeferred));
    }

    private void OnPaused(bool paused)
    {
        mayMove = !paused;
    }

    private void SetPlayerCharacterDeferred()
    {
        if (!Multiplayer.IsServer()) return;
        player.Manager.SetPlayerCharacter(player.PlayerId, player);
    }

    private async void DisableBlackScreenDeferred()
    {
        await ToSignal(GetTree(), "process_frame");
        var blackScreen = GetNode<ColorRect>("/root/Main/Level/Scene/canvas/blackScreen");
        blackScreen.Visible = false;
    }
    
    public override void _Process(double delta)
    {
        UpdateMoving();
        UpdateRunning();
        UpdateSitting();
        UpdateThiefControls();
    }

    private void UpdateMoving()
    {
        var inputDir = Input.GetVector(
            "ui_left", "ui_right",
            "ui_up", "ui_down"
        );

        if (!mayMove)
        {
            inputDir = Vector2.Zero;
        }
        
        player.Direction = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
    }

    private void UpdateRunning()
    {
        if (player.Team != Team.Thief) return;
        player.IsRunning = Input.IsActionPressed("ui_run");
    }

    private void UpdateSitting()
    {
        if (direction.Length() > 0)
        {
            player.IsSitting = false;
            return;
        }

        if (Input.IsActionJustPressed("ui_sit"))
        {
            player.IsSitting = !player.IsSitting;
        }
    }

    private void UpdateThiefControls()
    {
        if (player.Team != Team.Thief) return;
        if (player.IsTied) return;
        if (!Input.IsActionJustPressed("ui_click")) return;
        
        var clickedObject = Cursor.GetClickedObject(player);
        CheckClickToCharacter(clickedObject);
        CheckClickToCake(clickedObject);
    }

    private void CheckClickToCharacter(Node clickedObject)
    {
        if (clickedObject is not Character character) return;
        if (character == player) return;
        if (player.SpriteCode == character.SpriteCode) return;
        if (player.GlobalPosition.DistanceTo(character.GlobalPosition) > MinInteractDistance) return;

        player.Rpc(nameof(Character.LoadSprite), character.SpriteCode, true);
    }

    private void CheckClickToCake(Node clickedObject)
    {
        if (clickedObject is not Cake cake) return;
        if (player.GlobalPosition.DistanceTo(cake.GlobalPosition) > MinInteractDistance) return;
        cake.Rpc(nameof(Cake.Pick));
    }
}
