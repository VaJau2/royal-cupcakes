using Godot;

namespace RoyalCupcakes.Interface;

public partial class LogsLabel : RichTextLabel
{
	private const double MinShowTimer = 2f;
	private double timer;

	private static LogsLabel instance;
	public static LogsLabel GetInstance()
	{
		return instance;
	}
	
	public void AddMessage(string message)
	{
		Text += message + "\n";
		Visible = true;
		Modulate = Colors.White;
		timer = MinShowTimer + message.Length / 2.0;
		SetProcess(true);
	}

	public override void _Ready()
	{
		instance = this;
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		if (timer > 0)
		{
			timer -= delta;
			return;
		}
		
		if (Modulate.A > 0)
		{
			SetOpacity(-delta);
			return;
		}

		Visible = false;
		SetProcess(false);
	}

	private void SetOpacity(double deltaOpacity)
	{
		Modulate = new Color(1, 1, 1, Modulate.A + (float)deltaOpacity);
	}
}
