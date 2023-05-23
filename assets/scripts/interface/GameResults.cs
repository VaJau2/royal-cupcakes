using System.Collections.Generic;
using Godot;
using Godot.Collections;
using RoyalCupcakes.System;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Interface;

public partial class GameResults : Node3D
{
    [Export] private Array<Texture2D> thievesWin;
    [Export] private Array<Texture2D> guardsWin;
    
    private Main main;
    private Label resultsLabel;
    private Label continueLabel;
    private TextureRect picture;

    private Randomizer rand = new();

    public override void _Ready()
    {
        main = GetNode<Main>("/root/Main");
        continueLabel = GetNode<Label>("back/continueLabel");
        resultsLabel = GetNode<Label>("back/label");
        picture = GetNode<TextureRect>("back/picture");
        
        continueLabel.Visible = Multiplayer.IsServer();
        resultsLabel.Text = GetResultText();
        picture.Texture = GetResultPicture();
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

    private Texture2D GetResultPicture()
    {
        return main.WinnersTeam switch
        {
            Team.Guard => GetRandomPic(guardsWin),
            Team.Thief => GetRandomPic(thievesWin),
            _ => null
        };
    }

    private Texture2D GetRandomPic(IReadOnlyList<Texture2D> pics)
        => pics.Count > 0 
            ? pics[rand.GetInt(0, pics.Count)] 
            : null;

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void RestartLobby()
    {
        main.ChangeScene(null);
        main.ChangeMenu("lobby");
    }
}
