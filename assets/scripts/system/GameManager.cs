using Godot;
using RoyalCupcakes.Interface;

namespace RoyalCupcakes.System;

/**
 * Управляет игровыми правилами
 */
public partial class GameManager : Node
{
    private const double FinishingTimer = 2f;
    
    public static GameManager Instance { get; private set; }
    
    private Main main;
    private bool isLoaded;
    private bool isFinishing;
    private int guardsLeft, cakesLeft, thievesLeft;

    [Export]
    public double MainTimer { get; private set; }

    [Export] 
    public int GuardsLeft
    {
        get => guardsLeft;
        set
        {
            guardsLeft = value;
            if (value <= 0)
            {
                FinishGame(Team.Thief);
            }
        }
    }
    
    [Export] 
    public int ThievesLeft
    {
        get => thievesLeft;
        set
        {
            thievesLeft = value;
            if (value <= 0)
            {
                FinishGame(Team.Guard);
            }
        }
    }
    
    [Export] 
    public int CakesLeft
    {
        get => cakesLeft;
        set
        {
            cakesLeft = value;
            if (value <= 0)
            {
                FinishGame(Team.Thief);
            }
        }
    }
    
    public override void _Ready()
    {
        Instance = this;
        main = GetNode<Main>("../../../");
        CallDeferred(nameof(SetLoadedDeferred));

        if (!Multiplayer.IsServer()) return;
        MainTimer = Settings.Instance.MainTimer;
    }

    public override void _Process(double delta)
    {
        UpdateMainTimer(delta);
        ShowMainTimer((int)MainTimer);
    }

    private void UpdateMainTimer(double delta)
    {
        if (!Multiplayer.IsServer()) return;
        
        if (MainTimer > 0)
        {
            MainTimer -= delta;
            return;
        }
        
        FinishGame(Team.Guard);
    }

    private async void SetLoadedDeferred()
    {
        await ToSignal(GetTree(), "process_frame");
        isLoaded = true;
    }
    
    private async void FinishGame(Team winners)
    {
        if (!isLoaded) return;
        if (isFinishing) return;

        isFinishing = true;
        Rpc(nameof(SyncWinnersTeam), (int)winners);
        await ToSignal(GetTree().CreateTimer(FinishingTimer), "timeout");

        main.ChangeScene("game_results");
    }

    private void ShowMainTimer(int timer)
    {
        if (timer == 0) return;
        if (main.PlayerTeam != Team.Thief) return;
        if (MainLabel.Instance.isCountingTimer) return;

        var timeText = TranslationServer.Translate("#TIME_LEFT#").ToString();
        timeText = timeText.Replace("{time}", timer.ToString());
        MainLabel.Instance.ShowTempText(timeText, 1);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RequestAppendMainTimer()
    {
        if (!Multiplayer.IsServer()) return;
        MainTimer += Settings.Instance.AppendMainTimer;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RequestRemoveThief()
    {
        if (!Multiplayer.IsServer()) return;
        ThievesLeft--;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void SyncWinnersTeam(int winners)
    {
        main.WinnersTeam = (Team)winners;
    }
}
