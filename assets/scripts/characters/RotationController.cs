using Godot;
using Godot.Collections;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters;

public partial class RotationController : Node
{
	private const double RotationSpeed = 1.5;
	
	private double currentRotation;
	private double targetRotation;
	private double rotationDelta;
	private Array<Node3D> characters;

	public override void _Ready()
	{
		CallDeferred(nameof(LoadDeferred));
		SetProcess(false);
	}

	private async void LoadDeferred()
	{
		await ToSignal(GetTree(), "process_frame");
		var characterNodes = GetTree().GetNodesInGroup("character");
		characters = ArrayUtils.ConvertTo<Node3D>(characterNodes);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey) return;
		
		if (Input.IsActionJustPressed("ui_rotate_left"))
		{
			UpdateTargetRotation(true);
			SetProcess(true);
		}

		if (Input.IsActionJustPressed("ui_rotate_right"))
		{
			UpdateTargetRotation(false);
			SetProcess(true);
		}
	}

	private void UpdateTargetRotation(bool left)
	{
		rotationDelta = 0;
		targetRotation += left ? -90 : 90;
	}

	public override void _Process(double delta)
	{
		if (rotationDelta < 1)
		{
			rotationDelta += RotationSpeed * delta;
			currentRotation = Mathf.Lerp(currentRotation, targetRotation, rotationDelta);
		}
		else
		{
			currentRotation = targetRotation;
		}

		foreach (var character in characters)
		{
			var characterRotation = character.RotationDegrees;
			characterRotation.Y = (float)currentRotation;
			character.RotationDegrees = characterRotation;
		}

		if (rotationDelta < 1.5) return;
		SetProcess(false);
	}
}
