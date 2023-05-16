using Godot;

namespace RoyalCupcakes.Characters.Lasso;

/**
 * Рисует 3D линию между стартовой и конечной точкой
 */
public partial class RopeHandler : Node3D
{
	public void SetPoints(Vector3 start, Vector3 end)
	{
		if (start == end)
		{
			Visible = false;
			return;
		}

		Visible = true;
		GlobalPosition = start;
		LookAt(end, Vector3.Up);
		Scale = new Vector3(Scale.X, Scale.Y, GlobalPosition.DistanceTo(end));
	}
}
