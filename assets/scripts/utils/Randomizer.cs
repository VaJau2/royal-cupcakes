using System;
using Godot;

namespace RoyalCupcakes.Utils;

public class Randomizer
{ 
    private readonly Random rand = new();

    public int GetInt(int max) => rand.Next(max);
    public int GetInt(int min, int max) => rand.Next(min, max);

    public float GetFloat() => (float)rand.NextDouble();
    public float GetFloat(float max) => (float)rand.NextDouble() % max;
    public float GetFloat(float min, float max) => (float)rand.NextDouble() % max + min;
    
    public Vector3 GetPositionAround(Vector3 originPos, float radius)
        => new(
            GetRandomSide(originPos.X, radius),
            originPos.Y,
            GetRandomSide(originPos.Z, radius)
        );
    
    private float GetRandomSide(float originSide, float radius)
        => (float)(originSide + (rand.NextDouble() - 0.5f) * radius);
}
