using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectionModal : Control
{
	private const string HostCode = "#HOST#";
	private const string ClientCode = "#CLIENT#";
	private const string DefaultHost = "localhost";
	private const int DefaultPort = 8080;

	private Main main;

	[Export]
	private ConnectingModal connectingModal;
	[Export]
	private Control errorModal;
	
	private Button connectionType;
	private LineEdit ip;
	private LineEdit port;

	private bool isHost;

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");

		connectionType = GetNode<Button>("type");
		ip = GetNode<LineEdit>("ip");
		port = GetNode<LineEdit>("port");

		Multiplayer.ConnectedToServer += () =>
		{
			var mainMenu = GetNode<Control>("/root/Main/UI/MainMenu");
			mainMenu.QueueFree();
		};
	}

	public void OpenModal()
	{
		Visible = true;
	}

	private void OnConnectionTypePressed()
	{
		isHost = !isHost;
		ip.Visible = !isHost;
		connectionType.Text = isHost ? HostCode : ClientCode;
	}

	private void CloseConnectionModal()
	{
		Visible = false;
	}

	private void OnConnectPressed()
	{
		CloseConnectionModal();
		connectingModal.Visible = true;
		
		var settings = Settings.Instance;
		settings.Host = !string.IsNullOrEmpty(ip.Text) ? ip.Text : DefaultHost;
		settings.Port = !string.IsNullOrEmpty(port.Text) ? int.Parse(port.Text) : DefaultPort;

		main.Connect(settings, isHost);
	}
}
