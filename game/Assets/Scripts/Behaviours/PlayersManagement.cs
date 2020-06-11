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

    void OnEnable()
    {
        socket = GetComponent<SocketIOComponent>();
        PassObjectsToController();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupSocketEventListeners();
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
        socket.On(SocketEvents.SocketOpen, controller.OnConnectionOpen);
        socket.On(SocketEvents.PlayerNew, controller.OnPlayerAdded);
        socket.On(SocketEvents.PlayerGone, controller.OnPlayerGone);
        socket.On(SocketEvents.PlayerOtherPlayers, controller.OnOtherPlayersReceived);

        Debug.Log("Socket configured");
    }

    public GameObject GetLocalPlayer()
    {
        return controller.GetLocalPlayer();
    }

    internal void SendPlayerMove(Vector3 position, float horizontal, float vertical)
    {
        controller.SendPlayerMove(position.x, position.y, horizontal, vertical);
    }
}
