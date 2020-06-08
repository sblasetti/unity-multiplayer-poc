﻿using UnityEngine;

public class Move : MonoBehaviour
{
    GameObject localPlayer;
    Rigidbody rb;

    public float force = 50f;
    public float rotationSpeed = 100f;
    public ForceMode forceMode = ForceMode.Force;
    Vector3 direction;

    void Start()
    {
        var playersMgmt = this.GetComponent<PlayersManagement>();
        localPlayer = playersMgmt.GetLocalPlayer();
        direction = localPlayer.transform.forward;
        rb = localPlayer.GetComponent<Rigidbody>();
        Debug.Log($"forward: {localPlayer.transform.forward}");
        Debug.Log($"up: {localPlayer.transform.up}");
        Debug.Log($"right: {localPlayer.transform.right}");
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        direction = new Vector3(horizontal, 0, vertical);

        // TODO: this is a temp way to reset player's position, remove
        if (Input.GetKeyUp(KeyCode.R))
        {
            localPlayer.transform.SetPositionAndRotation(Vector3.up, new Quaternion(0, 0, 0, 0));
        }
    }

    void FixedUpdate()
    {
        var horizontal = direction.x;
        var vertical = direction.z;

        #region Using physics (Rigidbody)

        if (vertical != 0)
        {
            var v = vertical * force * Time.deltaTime;
            // rb.AddForce(localPlayer.transform.forward * v, forceMode);
            // rb.velocity = localPlayer.transform.forward * vertical * force;
            rb.MovePosition(localPlayer.transform.position + (localPlayer.transform.forward * v));
        }

        // if (horizontal != 0) {
        //     var h = horizontal * rotationSpeed * Time.deltaTime;
        //     rb.AddTorque(localPlayer.transform.up * h);
        // }

        #endregion

        #region Using Transform (does not work with physics / collisions)

        // if (vertical != 0) {
        //     var v = vertical * force * Time.deltaTime;
        //     localPlayer.transform.Translate(Vector3.forward * v);
        // }

        if (horizontal != 0)
        {
            var h = horizontal * rotationSpeed * Time.deltaTime;
            localPlayer.transform.Rotate(Vector3.up, h);
        }

        #endregion

        Debug.DrawRay(localPlayer.transform.position, localPlayer.transform.forward, Color.red);

        if (horizontal == 0 && vertical == 0)
            return;

        Debug.Log("send movement over network");

        var playersMgmt = this.GetComponent<PlayersManagement>();
        playersMgmt.SendPlayerMove(localPlayer.transform.position, horizontal, vertical);
    }
}
