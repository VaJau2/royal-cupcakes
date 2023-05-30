using Godot;
using RoyalCupcakes.Interface.Modals;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MainMenu : Control
{
	private AudioStreamPlayer audi;
	
	private ConnectionModal connectionModal;
	private Control errorModal;

	public override void _Ready()
	{
		Settings.Instance.LoadLanguage();
		
		connectionModal = GetNode<ConnectionModal>("connection");
		errorModal = GetNode<Control>("error");
		audi = GetNode<AudioStreamPlayer>("audi");
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

	public void ShowError()
	{
		errorModal.Visible = true;
	}

	private void OnExitPressed()
	{
		audi.Play();
		Settings.Instance.Save();
		GetTree().Quit();
	}
}
