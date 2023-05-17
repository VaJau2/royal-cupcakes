using Godot;
using Godot.Collections;

namespace RoyalCupcakes.Utils;

public static class Cursor
{
    private const int MaxMouseRayDistance = 1000;
    
    public static Vector3 GetCursorWorldPosition(Node3D parent)
    {
        var rayData = GetCursorRayData(parent);
        if (rayData.ContainsKey("position"))
        {
            return (Vector3)rayData["position"];
        }

        return Vector3.Zero;
    }

    public static Node GetClickedObject(Node3D parent)
    {
        var rayData = GetCursorRayData(parent);
        if (!rayData.ContainsKey("collider")) return null;
        var collider = (Node)rayData["collider"];
        return collider;
    }

    private static Dictionary GetCursorRayData(Node3D parent)
    {
        var spaceState = parent.GetWorld3D().DirectSpaceState;
        var mousePos = parent.GetViewport().GetMousePosition();
        var camera = parent.GetTree().Root.GetCamera3D();
        var rayOrigin = camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + camera.ProjectRayNormal(mousePos) * MaxMouseRayDistance;
        return spaceState.IntersectRay(PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd));
    }
}