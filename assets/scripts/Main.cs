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

	private CanvasLayer menuParent; 
	
	private Control currentMenu;
	private Node3D currentScene;

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			Settings.Instance.Save();
		}
	}

	public bool Connect(Settings settings, bool isHost)
	{
		if (isHost)
		{
			peer.CreateServer(settings.Port);
		}
		else
		{
			peer.CreateClient(settings.Host, settings.Port);
		}

		Multiplayer.MultiplayerPeer = peer;
		return peer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Disconnected;
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
		if (currentScene != null)
		{
			currentScene.QueueFree();
			currentScene = null;
		}

		if (string.IsNullOrEmpty(scene)) return;
		
		var scenePrefab = GD.Load<PackedScene>($"res://scenes/{scene}.tscn");
		var newScene = scenePrefab.Instantiate<Node3D>();
		AddChild(newScene);
		currentScene = newScene;
	}

	public override void _Ready()
	{
		menuParent = GetNode<CanvasLayer>("UI");
		currentMenu = menuParent.GetNode<Control>("MainMenu");
	}
}
