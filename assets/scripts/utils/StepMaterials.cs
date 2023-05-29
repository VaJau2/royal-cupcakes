using System.Collections.Generic;

namespace RoyalCupcakes.Utils;

public static class StepMaterials
{
    private static readonly Dictionary<int, string> materials = new()
    {
        [1] = "stone",
        [2] = "carpet",
    };

    public static string GetMaterial(int friction)
    {
        return materials[friction];
    }
}