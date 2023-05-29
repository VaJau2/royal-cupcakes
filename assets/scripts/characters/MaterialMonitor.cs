using Godot;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters;

public partial class MaterialMonitor : RayCast3D
{
	[Export] private SoundSteps soundSteps;

	public override void _Process(double delta)
	{
		if (!IsColliding()) return;

		var collideObj = GetCollider();
		if (collideObj is not StaticBody3D { PhysicsMaterialOverride: { } } collideBody) return;
		
		var friction = (int)collideBody.PhysicsMaterialOverride.Friction;
		var materialName = StepMaterials.GetMaterial(friction);

		soundSteps.SetMaterial(materialName);
	}
}
