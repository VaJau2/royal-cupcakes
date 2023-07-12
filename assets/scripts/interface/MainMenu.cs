using Godot;
using RoyalCupcakes.Interface.Modals;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MainMenu : Control
{
	private AudioStreamPlayer audi;
	
	private ConnectionModal connectionModal;
	private DebugModal debugModal;
	private ErrorModal errorModal;
	private Control settingsModal;

	public override void _Ready()
	{
		Settings.Instance.LoadLanguage();
		
		connectionModal = GetNode<ConnectionModal>("connection");
		debugModal = GetNode<DebugModal>("debug");
		errorModal = GetNode<ErrorModal>("error");
		settingsModal = GetNode<Control>("settings");
		audi = GetNode<AudioStreamPlayer>("audi");
	}
	
	public void ShowError()
	{
		errorModal.Visible = true;
	}

	private void OnLangChangePressed()
	{
		audi.Play();
		Settings.Instance.SetNextLanguage();
	}

	private void OnStartPressed()
	{
		audi.Play();
		if (string.IsNullOrEmpty(Settings.Instance.PlayerName))
		{
			errorModal.ErrorText = "#NAME_ERROR#";
			errorModal.OpenModal();
		}
		else
		{
			connectionModal.OpenModal();
		}
	}

	private void OnDebugPressed()
	{
		audi.Play();
		debugModal.OpenModal();
	}

	private void OnSettingsPressed()
	{
		audi.Play();
		settingsModal.Visible = true;
	}

	private void OnExitPressed()
	{
		audi.Play();
		Settings.Instance.Save();
		GetTree().Quit();
	}
}
