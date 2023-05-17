using Godot;

namespace RoyalCupcakes.Interface;

public partial class MainLabel : Label
{
	private bool isShowingText;
	private double tempTimer;
	
	public static MainLabel Instance { get; private set; }

	public void ShowText(string text)
	{
		isShowingText = true;
		Text = text;
	}

	public void HideText()
	{
		isShowingText = false;
		Text = "";
	}

	public void ShowTempText(string text, float timer)
	{
		if (isShowingText) return;
		Text = text;
		tempTimer = timer;
		SetProcess(true);
	}

	public override void _Ready()
	{
		Instance = this;
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		if (tempTimer > 0)
		{
			tempTimer -= delta;
		}
		else
		{
			HideText();
			SetProcess(false);
		}
	}
}
