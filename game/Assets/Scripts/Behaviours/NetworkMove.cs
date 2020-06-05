using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMove : MonoBehaviour
{
    Rigidbody playerRigidbody;
    void Start()
    {
        playerRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (horizontal == 0 && vertical == 0)
            return;

        Debug.Log("send movement over network");
    }
}
