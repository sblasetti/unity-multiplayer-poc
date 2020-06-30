using UnityEngine;

public class RaycastResult
{
    public bool Hit { get; set; }
    public GameObject ColliderGameObject { get; set; }
}

public interface IUnityPhysicsProxy
{
    RaycastResult Raycast(Vector3 origin, Vector3 direction, float maxLength);
}

public class RealUnityPhysicsProxy : IUnityPhysicsProxy
{
    public RaycastResult Raycast(Vector3 origin, Vector3 direction, float maxLength)
    {
        RaycastHit hitInfo;
        var hit = Physics.Raycast(origin, direction, out hitInfo, maxLength);

        return new RaycastResult
        {
            Hit = hit,
            ColliderGameObject = hitInfo.collider?.gameObject
        };
    }
}
