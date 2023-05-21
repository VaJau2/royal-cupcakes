using System.Diagnostics;
using Godot;
using RoyalCupcakes.Characters.Spawners;
using RoyalCupcakes.Interface;
using RoyalCupcakes.Props;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.System;

/**
 * Управляет игровыми правилами
 */
public partial class GameManager : Node3D
{
    private const double FinishingTimer = 2f;
    private const double SyncMainTimerDelay = 1f;

    private Randomizer rand = new();
    private Main main;
    private bool isLoaded, isFinishing;
    private int guardsLeft, cakesLeft, thievesLeft;
    private double syncMainTimer;

    private int startNpcCount, startCakesCount;

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

    public void HandleDisconnectedPlayer(Team playerTeam)
    {
        if (playerTeam == Team.Guard)
        {
            guardsLeft--;
        }
        else
        {
            thievesLeft--;
        }

        if (guardsLeft > 0 && thievesLeft > 0) return;
        main.ChangeMenu("lobby");
        main.ChangeScene(null);
    }
    
    public override void _EnterTree()
    {
        if (!Multiplayer.IsServer()) return;
        
        MainTimer = Settings.Instance.MainTimer;
        startCakesCount = Settings.Instance.CakesCount;
        startNpcCount = Settings.Instance.NpcCount;
        
        UpdateCakesSpawnersCount();
        UpdateNpcSpawnersCount();
    }

    private void UpdateCakesSpawnersCount()
    {
        var cakesParent = GetNode("cakeSpawners");
        var spawnersToDelete = cakesParent.GetChildCount() - startCakesCount;
        while (spawnersToDelete > 0)
        {
            var randI = rand.GetInt(0, cakesParent.GetChildCount());
            var spawner = cakesParent.GetChild(randI) as CakeSpawner;
            Debug.Assert(spawner != null, nameof(spawner) + " is not CakeSpawner");
            spawner.MaySpawn = false;
            spawnersToDelete -= 1;
        }
    }

    private void UpdateNpcSpawnersCount()
    {
        var npcSpawnersNodes = GetNode("npcSpawners").GetChildren();
        var npcSpawners = ArrayUtils.ConvertTo<NpcSpawner>(npcSpawnersNodes);
        var spawnerI = 0;
        
        while (startNpcCount > 0)
        {
            var spawner = npcSpawners[spawnerI];
            if (spawner.SpawnCount < spawner.MaxCount)
            {
                spawner.SpawnCount += 1;
                startNpcCount -= 1;
            }
            else
            {
                npcSpawners.RemoveAt(spawnerI);
            }
            
            if (npcSpawners.Count == 0)
            {
                break;
            }

            spawnerI = rand.GetInt(0, npcSpawners.Count);
        }
    }
    
    public override void _Ready()
    {
        main = GetNode<Main>("/root/Main");
        CallDeferred(nameof(SetLoadedDeferred));
    }
    
    private async void SetLoadedDeferred()
    {
        await ToSignal(GetTree(), "process_frame");
        isLoaded = true;
    }

    public override void _Process(double delta)
    {
        UpdateMainTimer(delta);
        ShowMainTimer((int)MainTimer);
    }

    private void UpdateMainTimer(double delta)
    {
        if (!Multiplayer.HasMultiplayerPeer() || !Multiplayer.IsServer()) return;
        
        if (MainTimer > 0)
        {
            MainTimer -= delta;

            if (syncMainTimer > 0)
            {
                syncMainTimer -= delta;
                return;
            }

            syncMainTimer = SyncMainTimerDelay;
            Rpc(nameof(SyncMainTime), MainTimer);
            return;
        }
        
        FinishGame(Team.Guard);
    }

    private async void FinishGame(Team winners)
    {
        if (!isLoaded) return;
        Rpc(nameof(SyncWinnersTeam), (int)winners);
        
        if (isFinishing) return;
        isFinishing = true;
        
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
        MainTimer += Settings.Instance.AppendMainTime;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RequestRemoveThief()
    {
        if (!Multiplayer.IsServer()) return;
        ThievesLeft--;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RequestRemoveGuard()
    {
        if (!Multiplayer.IsServer()) return;
        GuardsLeft--;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void SyncWinnersTeam(int winners)
    {
        main.WinnersTeam = (Team)winners;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SyncMainTime(double time)
    {
        MainTimer = time;
    }
}
