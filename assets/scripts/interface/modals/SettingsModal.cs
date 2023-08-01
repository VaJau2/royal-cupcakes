using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

/**
 * Модалка настроек громкости
 */
public partial class SettingsModal : AbstractModal
{
	private HSlider soundSlider;
	private HSlider musicSlider;
	private ControlsModal controlsModal;

	public bool ControlsIsOpen => controlsModal.Visible;

	public override void _Ready()
	{
		soundSlider = GetNode<HSlider>("sound");
		musicSlider = GetNode<HSlider>("music");
		controlsModal = GetNode<ControlsModal>("controls");
		
		var settings = Settings.Instance;
		soundSlider.Value = settings.SoundVolume;
		musicSlider.Value = settings.MusicVolume;
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;
		if (controlsModal.Visible) return;
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Visible = false;
		}
	}

	private void OnSoundValueChanged(double value)
	{
		Settings.Instance.SoundVolume = (float)value;
	}

	private void OnMusicValueChanged(double value)
	{
		Settings.Instance.MusicVolume = (float)value;
	}

	private void OnControlsPressed()
	{
		controlsModal.OpenModal();
	}
}
