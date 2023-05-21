using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Props;

public partial class Cake : StaticBody3D
{
	[Export] public string CakeCode { get; set; }

	private const float TextTimer = 2f;
	private GameManager gameManager;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/Main/Level/Scene");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Pick()
	{
		var cakeName = TranslationServer.Translate(CakeCode).ToString();
		var text = TranslationServer.Translate("#CAKE_STOLEN#").ToString();
		text = text.Replace("{cake}", cakeName);
		MainLabel.Instance.ShowTempText(text, TextTimer);
		gameManager.RpcId(1, nameof(GameManager.RequestAppendMainTimer));
		if (!Multiplayer.IsServer()) return;
		gameManager.CakesLeft--;
		QueueFree();
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetGlobalPos(Vector3 newPos)
	{
		GlobalPosition = newPos;
	}
}
