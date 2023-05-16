using Godot;

namespace RoyalCupcakes.Utils;

public static class Cursor
{
    private const int MaxMouseRayDistance = 1000;
    
    public static Vector3 GetCursorWorldPosition(Node3D parent)
    {
        var spaceState = parent.GetWorld3D().DirectSpaceState;
        var mousePos = parent.GetViewport().GetMousePosition();
        var camera = parent.GetTree().Root.GetCamera3D();
        var rayOrigin = camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + camera.ProjectRayNormal(mousePos) * MaxMouseRayDistance;
        var rayArray = spaceState.IntersectRay(PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd));
        if (rayArray.ContainsKey("position"))
        {
            return (Vector3)rayArray["position"];
        }

        return Vector3.Zero;
    }
}