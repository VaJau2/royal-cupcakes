using Godot;
using RoyalCupcakes.Interface;

namespace RoyalCupcakes.Props;

public partial class Cake : StaticBody3D
{
	private const float TextTimer = 2f;
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Pick()
	{
		MainLabel.Instance.ShowTempText("#CAKE_STOLEN#", TextTimer);
		QueueFree();
	}
}
