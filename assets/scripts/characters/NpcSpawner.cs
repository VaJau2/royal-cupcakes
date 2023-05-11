using System.Diagnostics;
using Godot;
using Godot.Collections;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters;

/**
 * Спавнит неписей в рандомных точках и дает им путь из этих точек
 * Точки должны находиться внутри спавнера
 */
public partial class NpcSpawner : Node3D
{
    private const int MinPointsCount = 3;

    [Export] private PackedScene npcPrefab;
    [Export] private int count;
    [Export] private Array<string> spriteCodes;

    private Array<RandomPoint> points = new();

    private readonly Randomizer rand = new();

    public override void _Ready()
    {
        foreach (var node in GetChildren())
        {
            points.Add(node as RandomPoint);
        }

        Debug.Assert(points.Count >= MinPointsCount, $"points count must be >= {MinPointsCount}");

        for (var i = 0; i < count; i++)
        {
            SpawnNpc();
        }
    }

    private void SpawnNpc()
    {
        points.Shuffle();

        var npc = npcPrefab.Instantiate<NPC>();
        npc.SpriteCode = spriteCodes[rand.GetInt(spriteCodes.Count)];
        
        var pointsCount = rand.GetInt(MinPointsCount, points.Count);
        for (var i = 0; i < pointsCount; i++)
        {
            npc.AppendToPath(points[i]);
        }

        AddChild(npc);

        var spawnPoint = points[0];
        npc.GlobalPosition = rand.GetPositionAround(spawnPoint.GlobalPosition, spawnPoint.RandomRadius);
    }
}
