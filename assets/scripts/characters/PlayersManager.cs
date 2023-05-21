using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Characters;

/**
 * Выводит в логи отключившегося игрока и удаляет его паппет
 */
public partial class PlayersManager : Node3D
{
	private Main main;
	private GameManager gameManager;
	private static LogsLabel logsLabel => LogsLabel.GetInstance();
	
	public void SetPlayerCharacter(int id, Character character)
	{
		var playerData = main.PlayersData[id];
		playerData.character = character;
		main.PlayersData[id] = playerData;
	}

	public override void _EnterTree()
	{
		if (!Multiplayer.IsServer()) return;
		SetMultiplayerAuthority(Multiplayer.GetUniqueId());
	}

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		gameManager = GetParent<GameManager>();

		if (!IsMultiplayerAuthority()) return;

		Multiplayer.PeerDisconnected += id =>
		{
			var playerId = (int)id;
			var playerData = main.PlayersData[playerId];
			gameManager.HandleDisconnectedPlayer(playerData.character.Team);
			playerData.character.QueueFree();
			Rpc(nameof(LogPlayerDisconnected), playerData.name);
			main.PlayersData.Remove(playerId);
		};
	}

	public PlayerData GetPLayerData(int id)
	{
		return main.PlayersData[id];
	}

	[Rpc(CallLocal = true)]
	private void LogPlayerDisconnected(string playerName)
	{
		var message = TranslationServer.Translate("#LOG_DISCONNECTED#").ToString();
		message = message.Replace("{name}", playerName);
		logsLabel.AddMessage(message);
	}
}
