using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

/**
 * Используется в главном меню для сохранения имени в конфиг
 */
public partial class PlayerNameInput : LineEdit
{
	public override void _Ready()
	{
		Text = Settings.Instance.PlayerName;
	}

	private void OnNameChanged(string newName)
	{
		Settings.Instance.PlayerName = newName;
	}
}
