using Godot;
using Godot.Collections;
using RoyalCupcakes.Props;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters;

/**
 * Класс НПЦ
 * Ходит по рандомным точкам и стоит/сидит на них некоторое время
 */
public partial class NPC : Character
{
    private const double MinWaitingTime = 8, MaxWaitingTime = 20;
    private const double MaxWaitToSitTime = 7;
    private const float SitChance = 0.4f;

    [Export] private Array<NodePath> pointsPath = new();
    private Array<RandomPoint> points = new();
    
    private int tempNodeKey;

    private NavigationAgent3D agent;
    private readonly Randomizer rand = new();

    public void AppendToPath(RandomPoint point)
    {
        points.Add(point);
    }

    public override void _Ready()
    {
        base._Ready();

        if (!IsMultiplayerAuthority())
        {
            SetProcess(false);
            return;
        }

        if (pointsPath.Count > 0)
        {
            ArrayUtils.LoadPath(pointsPath, this, out points);
        }
        
        points.Shuffle();

        Team = Team.Npc;
        agent = GetNode<NavigationAgent3D>("agent");
        CallDeferred(nameof(ActorSetup));
    }

    private async void ActorSetup()
    {
        await ToSignal(GetTree(), "physics_frame");
        agent.NavigationFinished += CameToPlace;
        SetNextTarget();
    }

    private void SetNextTarget()
    {
        var newPoint = points[tempNodeKey];
        agent.TargetPosition = rand.GetPositionAround(newPoint.Position, newPoint.RandomRadius);
        tempNodeKey = tempNodeKey < points.Count - 1 ? tempNodeKey + 1 : 0;
    }

    private void CameToPlace()
    {
        Velocity = Vector3.Zero;
        if (rand.GetFloat() < SitChance)
        {
            var sitTime = rand.GetFloat() % MaxWaitToSitTime;
            GetTree().CreateTimer(sitTime).Timeout += () => { IsSitting = true; };
        }

        var waitingTime = rand.GetFloat() % MaxWaitingTime + MinWaitingTime;
        GetTree().CreateTimer(waitingTime).Timeout += SetNextTarget;
    }

    public override void _Process(double _delta)
    {
        if (IsTied) return;
        if (agent.IsNavigationFinished()) return;
        var direction = (agent.GetNextPathPosition() - GlobalPosition).Normalized();
        UpdateMoveDirection(direction);
    }
}