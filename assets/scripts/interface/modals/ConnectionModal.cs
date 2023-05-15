using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectionModal : Window
{
	private const string HostCode = "#HOST#";
	private const string ClientCode = "#CLIENT#";
	private const string DefaultHost = "localhost";
	private const int DefaultPort = 8080;

	private Main main;

	[Export]
	private ConnectingModal connectingModal;
	[Export]
	private Window errorModal;
	
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
	}

	public void OpenModal()
	{
		PopupCentered();
	}

	private void OnConnectionTypePressed()
	{
		isHost = !isHost;
		ip.Visible = !isHost;
		connectionType.Text = isHost ? HostCode : ClientCode;
	}

	private void CloseConnectionModal()
	{
		Hide();
	}

	private void OnConnectPressed()
	{
		CloseConnectionModal();
		connectingModal.PopupCentered();
		
		var settings = Settings.Instance;
		settings.Host = !string.IsNullOrEmpty(ip.Text) ? ip.Text : DefaultHost;
		settings.Port = !string.IsNullOrEmpty(port.Text) ? int.Parse(port.Text) : DefaultPort;

		main.Connect(settings, isHost,
			() =>
			{
				connectingModal.Hide();
				main.ChangeMenu("lobby");
			},
			() =>
			{
				errorModal.PopupCentered();
			}
		);
	}
}
