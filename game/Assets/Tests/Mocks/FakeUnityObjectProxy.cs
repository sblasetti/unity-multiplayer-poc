using UnityEngine;

public class FakeUnityObjectProxy : IUnityObjectProxy
{
    public void Destroy(GameObject gameObject)
    {
    }

    public GameObject Instantiate(GameObject gameObject)
    {
        return null;
    }
}