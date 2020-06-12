using UnityEngine;

public interface IUnityTimeProxy
{
    float deltaTime { get; }
}

public class RealUnityTimeProxy : IUnityTimeProxy
{
    public float deltaTime => Time.deltaTime;
}
