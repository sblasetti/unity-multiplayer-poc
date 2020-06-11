using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using Zenject;

public class RemoteMovement : MonoBehaviour
{
    public GameSceneState State;
    SocketIOComponent socket;

    [Inject]
    IRemoteMovementController controller;

    void OnEnable()
    {
        socket = GetComponent<SocketIOComponent>();
    }

    void Setup()
    {
        SetupSocketEventListeners();
    }

    void FixedUpdate()
    {
    }

    private void SetupSocketEventListeners()
    {
        socket.On(SocketEvents.PlayerRemoteMove, controller.OnRemotePlayerMovement);
    }
}
