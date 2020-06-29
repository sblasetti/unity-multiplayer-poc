using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using Zenject;

public class RemoteMovement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public GameSceneState State;
    SocketIOComponent socket;

    [Inject]
    IRemoteMovementController controller;

    void Awake()
    {
        socket = GetComponent<SocketIOComponent>();
        SetupSocketEventListeners();
        PassObjectsToController();
    }
    
    void Start()
    {
    }

    private void PassObjectsToController()
    {
        // Is this the best way to use Unity related objects?
        controller.SetState(State);
        controller.SetMovementSpeed(movementSpeed);
        controller.SetRotatonSpeed(rotationSpeed);
    }

    private void SetupSocketEventListeners()
    {
        socket.On(SOCKET_EVENTS.PlayerRemoteMove, controller.OnRemotePlayerMovement);
    }
}
