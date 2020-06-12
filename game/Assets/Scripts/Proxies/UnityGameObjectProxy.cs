using UnityEngine;

public interface IUnityGameObjectProxy
{
    GameObject Find(string name);
}
public class RealUnityGameObjectProxy : IUnityGameObjectProxy
{
    public GameObject Find(string name)
    {
        return UnityEngine.GameObject.Find(name);
    }
}
