using UnityEngine;

public interface IUnityDebugProxy
{
    void Log(string v);
}

public class RealUnityDebugProxy : IUnityDebugProxy
{
    public void Log(string str)
    {
        Debug.Log(str);
    }
}
