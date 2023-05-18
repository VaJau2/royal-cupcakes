using Godot;
using Godot.Collections;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Props;

public partial class CakeSpawner : Node3D
{
	public bool MaySpawn { get; set; } = true;
	
	[Export] private Node3D cakesParent;
	[Export] private Array<PackedScene> cakePrefabs;

	private Randomizer rand = new();

	public override void _EnterTree()
	{
		if (!Multiplayer.IsServer()) return;
		SetMultiplayerAuthority(Multiplayer.GetUniqueId());
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		if (!MaySpawn) return;
		if (cakePrefabs.Count == 0) return;

		var cakeI = rand.GetInt(0, cakePrefabs.Count);
		var prefab = cakePrefabs[cakeI];
		var cake = prefab.Instantiate();
		CallDeferred(nameof(LoadCakeAsyncData), cake);
	}
	
	private async void LoadCakeAsyncData(Node cake)
	{
		cakesParent.AddChild(cake, true);
		await ToSignal(GetTree(), "physics_frame");
		cake.Rpc(nameof(Cake.SetGlobalPos), GlobalPosition);
	}
}
