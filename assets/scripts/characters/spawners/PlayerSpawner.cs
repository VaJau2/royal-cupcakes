using System.Linq;
using Godot;
using Godot.Collections;
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
	private GameManager gameManager;

	public override void _Ready()
	{
		if (!Multiplayer.IsServer()) return;

		main = GetNode<Main>("/root/Main");
		gameManager = GetParent<GameManager>();

		foreach (var node in GetChildren())
		{
			points.Add(node as RandomPoint);
		}

		foreach (var playerData in
		         main.PlayersData.Where(playerData => playerData.Value.team == playerTypeFilter))
		{
			SpawnPlayer(playerData.Key, playerData.Value.team);
		}
	}

	private void SpawnPlayer(int playerId, Team playerTeam)
	{
		CountPlayerTeam(playerTeam);

		var player = playerPrefab.Instantiate<Character>();
		player.Name += $"_{playerId}";
		
		if (!Multiplayer.IsServer()) return;
		var spriteCode = spriteCodes[rand.GetInt(spriteCodes.Count)];
		player.SpriteCode = spriteCode;
		player.Team = playerTeam;

		playersParent.AddChild(player);
		
		points.Shuffle();
		var spawnPoint = points[0];
		var newPos = rand.GetPositionAround(spawnPoint.GlobalPosition, spawnPoint.RandomRadius);
		CallDeferred(nameof(SetPosDeferred), player, newPos);
	}

	private async void SetPosDeferred(Character player, Vector3 newPos)
	{
		await ToSignal(GetTree(), "process_frame");
		player.Rpc(nameof(Character.SetGlobalPos), newPos);
	}

	private void CountPlayerTeam(Team playerTeam)
	{
		switch (playerTeam)
		{
			case Team.Guard:
				gameManager.GuardsLeft++;
				break;
			case Team.Thief:
				gameManager.ThievesLeft++;
				break;
		}
	}
}
