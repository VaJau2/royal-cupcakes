using Godot;
using Godot.Collections;
using RoyalCupcakes.Characters.Lasso;
using RoyalCupcakes.Props;
using RoyalCupcakes.System;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters.Spawners;

/**
 * Спавнит игрока в рандомной точке
 * Точки должны находиться внутри спавнера
 */
public partial class PlayerSpawner : Node3D
{
	[Export] private PlayersManager playersParent;
	[Export] private Team playerTypeFilter;
	[Export] private PackedScene playerPrefab;
	[Export] private Array<string> spriteCodes;
	
	private Array<RandomPoint> points = new();

	private readonly Randomizer rand = new();
	private Main main;

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		foreach (var node in GetChildren())
		{
			points.Add(node as RandomPoint);
		}
		
		if (main.PlayerTeam != playerTypeFilter) return;
		
		if (Multiplayer.IsServer())
		{
			SpawnPlayer(Multiplayer.GetUniqueId(), main.PlayerTeam);
		}
		else
		{
			RpcId(0, nameof(RequestSpawnPlayer), Multiplayer.GetUniqueId(), (int)main.PlayerTeam);
		}
	}
	
	private void SpawnPlayer(int playerId, Team playerTeam)
	{
		CountPlayerTeam(playerTeam);
		
		var player = playerPrefab.Instantiate<Character>();
		player.Name += $"_{playerId}";

		playersParent.AddChild(player);
		CallDeferred(nameof(LoadPlayerAsyncData), player, (int)playerTeam);
	}

	private void CountPlayerTeam(Team playerTeam)
	{
		switch (playerTeam)
		{
			case Team.Guard:
				GameManager.Instance.GuardsLeft++;
				break;
			case Team.Thief:
				GameManager.Instance.ThievesLeft++;
				break;
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestSpawnPlayer(int playerId, int playerTeam)
	{
		SpawnPlayer(playerId, (Team)playerTeam);
	}

	private async void LoadPlayerAsyncData(Node player, int playerTeam)
	{
		await ToSignal(GetTree(), "physics_frame");
		
		points.Shuffle();
		var spawnPoint = points[0];
		var newPos = rand.GetPositionAround(spawnPoint.GlobalPosition, spawnPoint.RandomRadius);
		var spriteCode = spriteCodes[rand.GetInt(spriteCodes.Count)];
		
		player.Rpc(nameof(Character.LoadTeam), playerTeam);
		player.Rpc(nameof(Character.LoadSprite), spriteCode, false);
		player.Rpc(nameof(Character.SetGlobalPos), newPos);
		
		if ((Team)playerTeam == Team.Guard)
		{
			var lassoHandler = player.GetNode<LassoHandler>("lasso");
			var npcCount = Settings.Instance.CaughtNpcCount;
			lassoHandler.Rpc(nameof(LassoHandler.SetCaughtNpcLeft), npcCount);
		}
	}
}
