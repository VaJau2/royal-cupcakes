using Godot;
using Godot.Collections;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class MiniMap : Control
{
	private const float Zoom = 0.025f;
	private Control markersParent;
	private Sprite2D enemyMarker;

	private Vector2 gridScale;
	private Dictionary<Node3D, Sprite2D> markers = new();
	private Node3D player;
	
	public void SetVisible(Node3D newPlayer)
	{
		player = newPlayer;

		if (Multiplayer.IsServer())
		{
			if (!Settings.Instance.MinimapOn) return;
			SetVisibleFromHost();
		}
		else
		{
			RpcId(1, nameof(RequestHostMinimapSettings));
		}
	}

	public void SpawnMarker(Node3D character)
	{
		if (enemyMarker.Duplicate() is not Sprite2D newMarker) return;
		markersParent.AddChild(newMarker);
		newMarker.Visible = true;
		markers.Add(character, newMarker);
	}

	public override void _Ready()
	{
		markersParent = GetNode<Control>("mask");
		enemyMarker = markersParent.GetNode<Sprite2D>("enemy");

		var viewportZoom = GetViewportRect().Size * Zoom;
		gridScale = markersParent.GetRect().Size / viewportZoom;
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;
		foreach (var markerData in markers)
		{
			if (IsInstanceValid(markerData.Key))
			{
				UpdateMarker(markerData.Key);
			}
			else
			{
				markerData.Value.QueueFree();
				markers.Remove(markerData.Key);
			}
		}
	}

	private void UpdateMarker(Node3D character)
	{
		markers[character].Position = GetMarkerPos(character);
	}
	
	private Vector2 GetMarkerPos(Node3D character)
	{
		var character2DPos = Get2DPos(character.GlobalPosition);
		var player2DPos = Get2DPos(player.GlobalPosition);
		var posByPlayer = character2DPos - player2DPos;
		return posByPlayer * gridScale + markersParent.GetRect().Size / 2;
	}

	private static Vector2 Get2DPos(Vector3 originPos) => new(originPos.X, originPos.Z);

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestHostMinimapSettings()
	{
		if (!Settings.Instance.MinimapOn) return;
		var senderId = Multiplayer.GetRemoteSenderId();
		RpcId(senderId, nameof(SetVisibleFromHost));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SetVisibleFromHost()
	{
		Visible = true;
	}
}
