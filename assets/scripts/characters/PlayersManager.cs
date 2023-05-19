using System.Collections.Generic;
using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Characters;

/**
 * Выводит в логи отключившегося игрока и удаляет его паппет
 */
public partial class PlayersManager : Node3D
{
	private readonly Dictionary<int, PlayerData> playersData = new();
	private static LogsLabel logsLabel => LogsLabel.GetInstance();

	public override void _EnterTree()
	{
		if (!Multiplayer.IsServer()) return;
		SetMultiplayerAuthority(Multiplayer.GetUniqueId());
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;

		Multiplayer.PeerDisconnected += id =>
		{
			var playerId = (int)id;
			playersData[playerId].character.QueueFree();
			Rpc(nameof(LogPlayerDisconnected), playersData[playerId].name);
			playersData.Remove(playerId);
		};
	}

	public void SetPlayerCharacter(int id, Character character)
	{
		var playerData = GetPLayerData(id);
		playerData.character = character;
		playersData[id] = playerData;

		if (Multiplayer.GetUniqueId() == id)
		{
			SetPlayerName(id, Settings.Instance.PlayerName);
			return;
		}
		
		RpcId(id, nameof(RequestPlayerName));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestPlayerName()
	{
		var id = Multiplayer.GetUniqueId();
		var name = Settings.Instance.PlayerName;
		Rpc(nameof(SetPlayerName), id, name);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SetPlayerName(int id, string playerName)
	{
		var playerData = GetPLayerData(id);
		playerData.name = playerName;
		playersData[id] = playerData;
	}

	public PlayerData GetPLayerData(int id)
	{
		return playersData.ContainsKey(id) ? playersData[id] : new PlayerData();
	}

	[Rpc(CallLocal = true)]
	private void LogPlayerDisconnected(string playerName)
	{
		var message = TranslationServer.Translate("#LOG_DISCONNECTED#").ToString();
		message = message.Replace("{name}", playerName);
		logsLabel.AddMessage(message);
	}
}

public struct PlayerData
{
	public Character character;
	public string name;
}
