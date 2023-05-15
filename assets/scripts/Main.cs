using System;
using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

/**
 * Класс, лежащий в корневом узле
 * Отвечает за подключение и переход по сценам/меню
 */
public partial class Main : Node
{
	public ENetMultiplayerPeer peer = new();
	public Team PlayerTeam { get; set; } = Team.Thief;

	private CanvasLayer menuParent;
	private Node levelParent;
	
	private Control currentMenu;
	private Node3D currentScene;

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			Settings.Instance.Save();
		}
	}

	public void Connect(Settings settings, bool isHost, Action connected)
	{
		if (isHost)
		{
			peer.CreateServer(settings.Port);
			Multiplayer.MultiplayerPeer = peer;
			connected();
		}
		else
		{
			Multiplayer.ConnectedToServer += connected;
			peer.CreateClient(settings.Host, settings.Port);
			Multiplayer.MultiplayerPeer = peer;
		}
	}

	public void Disconnect()
	{
		peer.Close();
		Multiplayer.MultiplayerPeer = null;
	}

	public void ChangeMenu(string menuPath, bool isError = false)
	{
		var menuPrefab = GD.Load<PackedScene>($"res://objects/interface/{menuPath}.tscn");
		if (menuPrefab == null) return;

		var newMenu = menuPrefab.Instantiate<Control>();
		
		currentMenu.QueueFree();
		menuParent.AddChild(newMenu);
		currentMenu = newMenu;

		if (isError && currentMenu is MainMenu mainMenu)
		{
			mainMenu.ShowError();
		}
	}

	public void ChangeScene(string scene)
	{
		if (!Multiplayer.IsServer()) return;
		
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

		void FailFunc()
		{
			ChangeScene(null);
			ChangeMenu("main_menu", true);
		}

		Multiplayer.ServerDisconnected += FailFunc;
		Multiplayer.ConnectionFailed += FailFunc;
	}
}

public enum Team
{
	Npc,
	Thief,
	Guard,
}