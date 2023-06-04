using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class PauseMenu : Control
{
	private Control settingsModal;
	private Control back;
	private AnimationPlayer anim;
	private AudioStreamPlayer audi;
	private bool isOpened;
	
	[Signal]
	public delegate void OpenedEventHandler(bool open);

	public override void _Ready()
	{
		settingsModal = GetNode<Control>("settings");
		back = GetNode<Control>("back");
		anim = GetNode<AnimationPlayer>("anim");
		audi = GetNode<AudioStreamPlayer>("audi");
		anim.AnimationFinished += _ =>
		{
			if (!isOpened)
			{
				back.Visible = false;
			}
		};
	}

	public override void _Process(double delta)
	{
		if (!Input.IsActionJustPressed("ui_cancel")) return;
		SetOpen(!isOpened);
	}

	private void OnContinuePressed()
	{
		audi.Play();
		SetOpen(false);
	}

	private void OnSettingsPressed()
	{
		audi.Play();
		settingsModal.Visible = true;
	}

	private void OnExitPressed()
	{
		audi.Play();
		var main = GetNode<Main>("/root/Main");  
		main.Disconnect();
		main.ChangeScene(null);
		main.ChangeMenu("main_menu");
	}

	private void SetOpen(bool open)
	{
		anim.Play(!open ? "close" : "open");
		isOpened = open;
		
		if (isOpened)
		{
			back.Visible = true;
		}

		EmitSignal(nameof(Opened), isOpened);
	}
}
