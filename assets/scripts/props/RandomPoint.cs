using Godot;

namespace RoyalCupcakes.Props;

/**
 * Определяет радиус, в котором могут тусоваться неписи вокруг точки
 */
public partial class RandomPoint : Node3D
{
	[Export] public float RandomRadius { get; private set; } = 1f;
}
