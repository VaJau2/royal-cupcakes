using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class GameResults : Node3D
{
    private Main main;
    private Label resultsLabel;
    private Label continueLabel;

    public override void _Ready()
    {
        main = GetNode<Main>("/root/Main");
        continueLabel = GetNode<Label>("back/continueLabel");
        continueLabel.Visible = Multiplayer.IsServer();
        resultsLabel = GetNode<Label>("back/label");
        resultsLabel.Text = GetResultText();
    }

    public override void _Process(double delta)
    {
        if (!Multiplayer.IsServer()) return;
        if (!Input.IsActionJustPressed("ui_select")) return;
        Rpc(nameof(RestartLobby));
    }

    private string GetResultText()
    {
        return main.WinnersTeam switch
        {
            Team.Guard => "#GUARDS_WINNERS#",
            Team.Thief => "#THIEF_WINNERS#",
            _ => "#GAME_BROKEN#"
        };
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void RestartLobby()
    {
        main.ChangeScene(null);
        main.ChangeMenu("lobby");
    }
}
