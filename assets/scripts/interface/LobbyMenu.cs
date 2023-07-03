using System.Linq;
using Godot;
using Godot.Collections;
using RoyalCupcakes.System;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Interface;

/**
 * Меню со списком игроков
 * Валидирует количество игроков в командах и загружает игровую сцену
 */
public partial class LobbyMenu : Control
{
	private const string ReadyCode = "#READY#";
	private const string NotReadyCode = "#NOT_READY#";
	private const string NotEnoughPlayers = "#NOT_ENOUGH_PLAYERS#";
	private const string NotEveryoneReady = "#NOT_EVERYONE_READY#";
	private const string Starting = "#STARTING#";
	private const double StartingTimer = 1.0;
	
	private Main main;

	[Export] private PackedScene playerItemPrefab;
	private VBoxContainer listParent;

	private Button readyButton;
	private Button settingsButton;
	private Control settings;
	private Label messageLabel;
	private AudioStreamPlayer audi;

	private double timer;
	private Array<long> spawnedPlayers = new(); 

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		listParent = GetNode<VBoxContainer>("back/players");
		readyButton = GetNode<Button>("back/ready");
		settingsButton = GetNode<Button>("back/settings");
		settings = GetNode<Control>("settings");
		messageLabel = GetNode<Label>("back/message");
		audi = GetNode<AudioStreamPlayer>("audi");
		SetProcess(false);

		if (Multiplayer.IsServer())
		{
			AddAlreadyConnectedPlayers(Multiplayer.GetUniqueId());
			Multiplayer.PeerConnected += playerId =>
			{
				Rpc(nameof(AddPlayer), playerId);
			};
			Multiplayer.PeerDisconnected += playerId =>
			{
				Rpc(nameof(RemovePlayer), playerId);
			};
		}
		else
		{
			RpcId(1, nameof(RequestToSpawnPlayerItems));
			settingsButton.Visible = false;
			Multiplayer.ServerDisconnected += () =>
			{
				main.ChangeMenu("main_menu", true);
			};
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestToSpawnPlayerItems()
	{
		var senderId = Multiplayer.GetRemoteSenderId();
		AddAlreadyConnectedPlayers(senderId);
	}

	private void AddAlreadyConnectedPlayers(long clientId)
	{
		var connectedPlayers = Multiplayer.GetPeers();
		connectedPlayers = connectedPlayers.Concat(new[] { Multiplayer.GetUniqueId() }).ToArray();

		foreach (var playerId in connectedPlayers)
		{
			if (clientId == Multiplayer.GetUniqueId())
			{
				AddPlayer(playerId);
			}
			else
			{
				RpcId(clientId, nameof(AddPlayer), playerId);
			}
		}
	}

	[Rpc(CallLocal = true)]
	private void AddPlayer(long playerId)
	{
		if (spawnedPlayers.Contains(playerId)) return;
		
		var playerItem = playerItemPrefab.Instantiate<LobbyPlayerItem>();
		playerItem.Name = playerId.ToString();
		listParent.AddChild(playerItem);
		spawnedPlayers.Add(playerId);
	}

	[Rpc(CallLocal = true)]
	private void RemovePlayer(long playerId)
	{
		if (!spawnedPlayers.Contains(playerId)) return;
		
		var playerItem = GetPlayerItem((int)playerId);
		playerItem.QueueFree();
		spawnedPlayers.Remove(playerId);
	}

	private void OnBackPressed()
	{
		audi.Play();
		main.Disconnect();
		main.ChangeMenu("main_menu");
	}

	private void OnSettingsPressed()
	{
		audi.Play();
		settings.Visible = !settings.Visible;
	}

	private void OnChangeTeamPressed()
	{
		audi.Play();
		var playerItem = GetPlayerItem(Multiplayer.GetUniqueId());
		main.PlayerTeam = main.PlayerTeam == Team.Guard ? Team.Thief : Team.Guard;
		playerItem.Team = main.PlayerTeam;
		playerItem.Synchronize();
	}

	private void OnReadyPressed()
	{
		audi.Play();
		var playerItem = GetPlayerItem(Multiplayer.GetUniqueId());
		playerItem.PlayerReady = !playerItem.PlayerReady;
		playerItem.Synchronize();

		readyButton.Text = playerItem.PlayerReady ? NotReadyCode : ReadyCode;
	}
	
	private LobbyPlayerItem GetPlayerItem(int playerId)
		=> listParent.GetNode<LobbyPlayerItem>(playerId.ToString());

	private void SetPlayersDataList()
	{
		main.PlayersData.Clear();
		
		var listNodes = listParent.GetChildren();
		var listPlayerItems = ArrayUtils.ConvertTo<LobbyPlayerItem>(listNodes);
		foreach (var playerItem in listPlayerItems)
		{
			main.PlayersData.Add(playerItem.PlayerId, new PlayerData
			{
				name = playerItem.PlayerName,
				team = playerItem.Team
			});
		}
	}

	public void CheckStartGame()
	{
		int readyPlayersCount = 0, 
			thievesCount = 0, guardsCount = 0;

		foreach (var node in listParent.GetChildren())
		{
			var playerItem = (LobbyPlayerItem)node;
			
			if (playerItem.PlayerReady) readyPlayersCount++;
			if (playerItem.Team == Team.Thief) thievesCount++;
			if (playerItem.Team == Team.Guard) guardsCount++;
		}

		if (thievesCount < 1 || guardsCount < 1)
		{
			messageLabel.Text = NotEnoughPlayers;
			SetProcess(false);
			return;
		}

		if (readyPlayersCount < thievesCount + guardsCount)
		{
			messageLabel.Text = NotEveryoneReady;
			SetProcess(false);
			return;
		}

		if (IsProcessing()) return;
		messageLabel.Text = Starting;
		timer = StartingTimer;
		SetProcess(true);
	}

	public override void _Process(double delta)
	{
		if (timer > 0)
		{
			timer -= delta;
			return;
		}

		SetPlayersDataList();
		main.ChangeMenu("pause_menu");
		main.ChangeScene("game");
		SetProcess(false);
	}
}
