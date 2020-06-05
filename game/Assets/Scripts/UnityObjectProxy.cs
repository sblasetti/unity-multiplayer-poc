using UnityEngine;

public interface IUnityObjectProxy
{
    GameObject Instantiate(GameObject gameObject);
    void Destroy(GameObject gameObject);
}
public class RealUnityObjectProxy : IUnityObjectProxy
{
    public void Destroy(GameObject gameObject)
    {
        UnityEngine.Object.Destroy(gameObject);
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
