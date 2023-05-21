using Godot;
using Godot.Collections;
using RoyalCupcakes.System;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Props;

public partial class CakeSpawner : Node3D
{
	private static int cakeGlobalCount;
	public bool MaySpawn { get; set; } = true;
	
	[Export] private Node3D cakesParent;
	[Export] private Array<PackedScene> cakePrefabs;

	private Randomizer rand = new();

	public override void _Ready()
	{
		if (!Multiplayer.IsServer()) return;
		if (cakePrefabs.Count == 0 || !MaySpawn) return;
		SpawnCake();
	}

	private void SpawnCake()
	{
		var chosenPrefab = rand.GetInt(0, cakePrefabs.Count);
		var prefab = cakePrefabs[chosenPrefab];
		var cake = prefab.Instantiate<Cake>();
		cake.Name += cakeGlobalCount++;
		cakesParent.AddChild(cake, true);
		CallDeferred(nameof(LoadCakeAsyncData), cake);

		var gameManager = GetNode<GameManager>("/root/Main/Level/Scene");
		gameManager.CakesLeft++;
	}
	
	private async void LoadCakeAsyncData(Node cake)
	{
		await ToSignal(GetTree(), "physics_frame");
		cake.Rpc(nameof(Cake.SetGlobalPos), GlobalPosition);
	}
}
