using System.Diagnostics;
using Godot;
using Godot.Collections;
using RoyalCupcakes.Props;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters.Spawners;

/**
 * Спавнит неписей в рандомных точках и дает им путь из этих точек
 * Точки должны находиться внутри спавнера
 */
public partial class NpcSpawner : Node3D
{
    private const int MinPointsCount = 3;
    private static int npcCount;

    [Export] public int MaxCount { get; private set; }
    public int SpawnCount { get; set; }
    
    [Export] private Node3D npcParent;
    [Export] private PackedScene npcPrefab;
    [Export] private Array<string> spriteCodes;

    private Array<RandomPoint> points = new();

    private readonly Randomizer rand = new();

    public override void _Ready()
    {
        if (!Multiplayer.IsServer()) return;
        
        foreach (var node in GetChildren())
        {
            points.Add(node as RandomPoint);
        }

        Debug.Assert(points.Count >= MinPointsCount, $"points count must be >= {MinPointsCount}");

        for (var i = 0; i < SpawnCount; i++)
        {
            SpawnNpc();
        }
    }

    private void SpawnNpc()
    {
        points.Shuffle();
        var spawnPoint = points[0];
        var newPos = rand.GetPositionAround(spawnPoint.GlobalPosition, spawnPoint.RandomRadius);
        var spriteCode = spriteCodes[rand.GetInt(spriteCodes.Count)];
        var npc = npcPrefab.Instantiate<NPC>();
        
        npc.Name += $"{++npcCount}_{Multiplayer.GetUniqueId()}";
        npc.SpriteCode = spriteCode;

        var pointsCount = rand.GetInt(MinPointsCount, points.Count);
        for (var i = 0; i < pointsCount; i++)
        {
            npc.AppendToPath(points[i]);
        }

        npcParent.AddChild(npc);
        npc.GlobalPosition = newPos;
    }
}
