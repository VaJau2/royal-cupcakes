using Godot;
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

	private double timer;

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		listParent = GetNode<VBoxContainer>("back/players");
		readyButton = GetNode<Button>("back/ready");
		settingsButton = GetNode<Button>("back/settings");
		settings = GetNode<Control>("settings");
		messageLabel = GetNode<Label>("back/message");
		SetProcess(false);

		if (Multiplayer.IsServer())
		{
			AddPlayer(Multiplayer.GetUniqueId());
			AddAlreadyConnectedPlayers();
			Multiplayer.PeerConnected += AddPlayer;
			Multiplayer.PeerDisconnected += RemovePlayer;
		}
		else
		{
			settingsButton.Visible = false;
			Multiplayer.ServerDisconnected += () =>
			{
				main.ChangeMenu("main_menu", true);
			};
		}
	}

	private void AddAlreadyConnectedPlayers()
	{
		foreach (var playerId in Multiplayer.GetPeers())
		{
			AddPlayer(playerId);
		}
	}

	private void AddPlayer(long playerId)
	{
		var playerItem = playerItemPrefab.Instantiate<LobbyPlayerItem>();
		playerItem.Name = playerId.ToString();
		listParent.AddChild(playerItem);
	}

	private void RemovePlayer(long playerId)
	{
		if (!Multiplayer.IsServer()) return;
		var playerItem = GetPlayerItem((int)playerId);
		playerItem.QueueFree();
	}

	private void OnBackPressed()
	{
		main.Disconnect();
		main.ChangeMenu("main_menu");
	}

	private void OnSettingsPressed()
	{
		settings.Visible = !settings.Visible;
	}

	private void OnChangeTeamPressed()
	{
		var playerItem = GetPlayerItem(Multiplayer.GetUniqueId());
		main.PlayerTeam = main.PlayerTeam == Team.Guard ? Team.Thief : Team.Guard;
		playerItem.Team = main.PlayerTeam;
		playerItem.OnSynchronize();
	}

	private void OnReadyPressed()
	{
		var playerItem = GetPlayerItem(Multiplayer.GetUniqueId());
		playerItem.PlayerReady = !playerItem.PlayerReady;
		playerItem.OnSynchronize();

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
