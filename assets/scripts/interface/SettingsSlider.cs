using Godot;
using System.Globalization;

public partial class SettingsSlider : HSlider
{
	private Label countLabel;
	
	public override void _Ready()
	{
		countLabel = GetNode<Label>("count");
		UpdateLabel(Value);
		ValueChanged += UpdateLabel;
	}

	private void UpdateLabel(double value)
	{
		countLabel.Text = value.ToString(CultureInfo.InvariantCulture);
	}
}
