using Godot;
using RoyalCupcakes.Interface.Modals;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MainMenu : Control
{
	private ConnectionModal connectionModal;
	private Control errorModal;

	public override void _Ready()
	{
		Settings.Instance.LoadLanguage();
		
		connectionModal = GetNode<ConnectionModal>("connection");
		errorModal = GetNode<Control>("error");
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
		errorModal.Visible = true;
	}

	private void OnExitPressed()
	{
		Settings.Instance.Save();
		GetTree().Quit();
	}
}
