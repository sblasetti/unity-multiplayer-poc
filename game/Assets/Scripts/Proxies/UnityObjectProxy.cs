using UnityEngine;

public interface IUnityObjectProxy
{
    GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation);
    void DestroyImmediate(GameObject gameObject);
}
public class RealUnityObjectProxy : IUnityObjectProxy
{
    public void DestroyImmediate(GameObject gameObject)
    {
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        return UnityEngine.Object.Instantiate(gameObject, position, rotation);
    }

    public void Log(string str)
    {
        Debug.Log(str);
    }
}
