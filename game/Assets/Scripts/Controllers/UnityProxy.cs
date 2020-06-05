using UnityEngine;

public interface IUnityProxy
{
    GameObject Instantiate(GameObject playerPrefab);
    void Destroy(GameObject player);
}
public class UnityProxy : IUnityProxy
{
    public void Destroy(GameObject gameObject)
    {
        UnityEngine.Object.Destroy(gameObject);
    }

    public GameObject Instantiate(GameObject gameObject)
    {
        return UnityEngine.Object.Instantiate(gameObject);
    }
}