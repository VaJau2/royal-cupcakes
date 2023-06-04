using Godot;
using RoyalCupcakes.Interface.Modals;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MainMenu : Control
{
	private AudioStreamPlayer audi;
	
	private ConnectionModal connectionModal;
	private Control errorModal;
	private Control settingsModal;

	public override void _Ready()
	{
		Settings.Instance.LoadLanguage();
		
		connectionModal = GetNode<ConnectionModal>("connection");
		errorModal = GetNode<Control>("error");
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
		connectionModal.OpenModal();
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
