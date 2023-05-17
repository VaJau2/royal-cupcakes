using Godot;

namespace RoyalCupcakes.Characters.Player;

/**
 * Класс контроллера висит на игроке и отвечает за управление им
 */
public partial class InputController : Node
{
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
}
