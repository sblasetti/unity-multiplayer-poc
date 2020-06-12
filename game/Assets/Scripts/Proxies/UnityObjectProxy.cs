using UnityEngine;

public interface IUnityObjectProxy
{
    GameObject Instantiate(GameObject gameObject);
    void DestroyImmediate(GameObject gameObject);
}
public class RealUnityObjectProxy : IUnityObjectProxy
{
    public void DestroyImmediate(GameObject gameObject)
    {
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    public GameObject Instantiate(GameObject gameObject)
    {
        return UnityEngine.Object.Instantiate(gameObject);
    }

    public void Log(string str)
    {
        Debug.Log(str);
    }
}
