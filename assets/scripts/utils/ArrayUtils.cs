using Godot;
using Godot.Collections;

namespace RoyalCupcakes.Utils;

public static class ArrayUtils
{
    public static void LoadPath<[MustBeVariant] T>(
        Array<NodePath> path, Node parent, out Array<T> nodes
    ) where T : class
    {
        nodes = new Array<T>();

        foreach (var pathItem in path)
        {
            nodes.Add(parent.GetNode<T>(pathItem));
        }
    }
}