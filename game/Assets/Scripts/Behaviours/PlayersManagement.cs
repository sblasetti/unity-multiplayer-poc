using UnityEngine;
using SocketIO;
using System;
using Zenject;

public class PlayersManagement : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameSceneState State;
    SocketIOComponent socket;

    [Inject]
    IPlayersManagementController controller;

    void Awake()
    {
        socket = GetComponent<SocketIOComponent>();
        SetupSocketEventListeners();
        PassObjectsToController();
    }

    private void PassObjectsToController()
    {
        // Is this the best way to use Unity related objects?
        controller.SetPlayerPrefab(playerPrefab);
        controller.SetSocket(socket);
        controller.SetState(State);
    }

    private void SetupSocketEventListeners()
    {
        socket.On(SOCKET_EVENTS.ServerWelcome, controller.OnPlayerWelcome);
        socket.On(SOCKET_EVENTS.PlayerNew, controller.OnPlayerAdded);
        socket.On(SOCKET_EVENTS.PlayerGone, controller.OnPlayerGone);
        socket.On(SOCKET_EVENTS.PlayerOtherPlayers, controller.OnOtherPlayersReceived);

        Debug.Log("Socket configured");
    }
}
