using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Класс контроллера висит на игроке и отвечает за управление им
 */
public partial class InputController : Node3D
{
    private Character player;
    private Vector3 direction;

    public override void _Ready()
    {
        player = GetParent<Character>();
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
        direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        player.UpdateMoveDirection(direction);
    }

    private void UpdateRunning()
    {
        if (player.type != Character.CharacterType.Thief) return;
        player.isRunning = Input.IsActionPressed("ui_run");
    }

    private void UpdateSitting()
    {
        if (direction.Length() > 0)
        {
            player.isSitting = false;
            return;
        }

        if (Input.IsActionJustPressed("ui_sit"))
        {
            player.isSitting = !player.isSitting;
        }
    }
}
