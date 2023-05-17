using Godot;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters.Player;

/**
 * Класс контроллера висит на игроке и отвечает за управление им
 */
public partial class InputController : Node
{
    private const float MinChangeSpriteDistance = 1f;
    
    private Character player;
    private Vector3 direction;

    public override void _Ready()
    {
        player = GetParent<Character>();

        if (!player.IsMultiplayerAuthority())
        {
            SetProcess(false);
            return;
        }
        
        var camera = GetNode<Camera3D>("../camera");
        camera.Current = true;
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
        direction = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        player.UpdateMoveDirection(direction);
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
        if (!Input.IsActionJustPressed("ui_click")) return;
        
        var clickedObject = Cursor.GetClickedObject(player);
        if (clickedObject is not Character character) return;
        if (character == player) return;
        if (player.SpriteCode == character.SpriteCode) return;
        if (player.GlobalPosition.DistanceTo(character.GlobalPosition) > MinChangeSpriteDistance) return;

        player.Rpc(nameof(Character.LoadSprite), character.SpriteCode, true);
    }
}
