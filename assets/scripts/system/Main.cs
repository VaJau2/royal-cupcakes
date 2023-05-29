using Godot;
using System.Collections.Generic;
using RoyalCupcakes.Characters;
using RoyalCupcakes.Interface;

namespace RoyalCupcakes.System;

/**
 * Класс, лежащий в корневом узле
 * Отвечает за подключение и переход по сценам/меню
 */
public partial class Main : Node
{
	public readonly Dictionary<int, PlayerData> PlayersData = new();
	public Team PlayerTeam { get; set; }
	public Team WinnersTeam { get; set; }

	private CanvasLayer menuParent;
	private Node levelParent;
	
	private Control currentMenu;
	private Node3D currentScene;
	
	private ENetMultiplayerPeer peer = new();
	
	[Export] private bool debugHost;

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			Settings.Instance.Save();
		}
	}

	public void Connect(Settings settings, bool isHost)
	{
		if (isHost)
		{
			peer.CreateServer(settings.Port);
			Multiplayer.MultiplayerPeer = peer;

			if (debugHost)
			{
				ChangeMenu("pause_menu");
				ChangeScene("game");
			}
			else
			{
				ChangeMenu("lobby");
			}
		}
		else
		{
			peer.CreateClient(settings.Host, settings.Port);
			Multiplayer.MultiplayerPeer = peer;
			currentMenu = null;
			currentScene = null;
		}
	}

	public void Disconnect()
	{
		peer.Close();
		Multiplayer.MultiplayerPeer = null;
	}

	public void ChangeMenu(string menuPath, bool isError = false)
	{
		if (Multiplayer.HasMultiplayerPeer() && !Multiplayer.IsServer()) return;

		var menuPrefab = GD.Load<PackedScene>($"res://objects/interface/{menuPath}.tscn");
		if (menuPrefab == null) return;

		var newMenu = menuPrefab.Instantiate<Control>();

		currentMenu?.QueueFree();
		menuParent.AddChild(newMenu);
		currentMenu = newMenu;

		if (isError && currentMenu is MainMenu mainMenu)
		{
			mainMenu.ShowError();
		}
	}

	public void ChangeScene(string scene)
	{
		if (Multiplayer.HasMultiplayerPeer() && !Multiplayer.IsServer()) return;
		
		if (currentScene != null)
		{
			currentScene.QueueFree();
			currentScene = null;
		}

		if (string.IsNullOrEmpty(scene)) return;
		
		var scenePrefab = GD.Load<PackedScene>($"res://scenes/{scene}.tscn");
		var newScene = scenePrefab.Instantiate<Node3D>();
		levelParent.AddChild(newScene);
		currentScene = newScene;
	}

	public override void _Ready()
	{
		menuParent = GetNode<CanvasLayer>("UI");
		levelParent = GetNode<Node>("Level");
		currentMenu = menuParent.GetNode<Control>("MainMenu");

		Multiplayer.ServerDisconnected += FailFunc;
		Multiplayer.ConnectionFailed += FailFunc;

		if (!debugHost) return;
		StartDebugHost();
	}

	private void FailFunc()
	{
		Multiplayer.MultiplayerPeer = null;
		ChangeScene(null);
		ChangeMenu("main_menu", true);
	}

	private void StartDebugHost()
	{
		PlayerTeam = Team.Thief;
		
		PlayersData.Add(1, new PlayerData
		{
			name = Settings.Instance.PlayerName,
			team = PlayerTeam
		});
		Connect(Settings.Instance, true);
	}
}

public enum Team
{
	Npc,
	Thief,
	Guard,
}

public struct PlayerData
{
	public Character character;
	public string name;
	public Team team;
}
