using Godot;
using RoyalCupcakes.Interface.Modals;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MainMenu : Control
{
	private ConnectionModal connectionModal;
	private Window errorModal;

	public override void _Ready()
	{
		connectionModal = GetNode<ConnectionModal>("connection");
		errorModal = GetNode<Window>("error");
	}

	private void OnLangChangePressed()
	{
		Settings.Instance.SetNextLanguage();
	}

	private void OnStartPressed()
	{
		connectionModal.OpenModal();
	}

	public void ShowError()
	{
		errorModal.PopupCentered();
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
