using Godot;
using RoyalCupcakes.System;

public partial class SettingsSubmenu : Control
{
	[Export] private HSlider cakesSlider;
	[Export] private HSlider npcSlider;
	[Export] private HSlider startTimeSlider;
	[Export] private HSlider appendTimeSlider;
	[Export] private HSlider mistakesSlider;

	public override void _EnterTree()
	{
		var settings = Settings.Instance;
		cakesSlider.Value = settings.CakesCount;
		npcSlider.Value = settings.NpcCount;
		startTimeSlider.Value = settings.MainTimer;
		appendTimeSlider.Value = settings.AppendMainTime;
		mistakesSlider.Value = settings.CaughtNpcCount;
	}

	private void OnClosePressed()
	{
		Visible = false;
	}

	private void OnCakesValueChanged(double value)
	{
		Settings.Instance.CakesCount = (int)value;
	}

	private void OnNpcValueChanged(double value)
	{
		Settings.Instance.NpcCount = (int)value;
	}

	private void OnStartTimeValueChanged(double value)
	{
		Settings.Instance.MainTimer = value;
	}

	private void OnAppendTimeValueChanged(double value)
	{
		Settings.Instance.AppendMainTime = value;
	}

	private void OnMistakesCountValueChanged(double value)
	{
		Settings.Instance.CaughtNpcCount = (int)value;
	}
}