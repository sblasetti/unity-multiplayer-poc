using UnityEngine;
using SocketIO;
using System;
using Zenject;

public class PlayersManagement : MonoBehaviour
{
    public GameObject playerPrefab;
    SocketIOComponent socket;

    [Inject]
    IPlayersManagementController controller;

    void OnEnable()
    {
        controller.SetPlayerPrefab(playerPrefab);
        controller.SetSocket(GetSocket());
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupSocketEventListeners();
    }

    private void SetupSocketEventListeners()
    {
        socket = GetSocket();

        socket.On(SocketEvents.SocketOpen, controller.OnConnectionOpen);
        socket.On(SocketEvents.PlayerNew, controller.OnPlayerAdded);
        socket.On(SocketEvents.PlayerGone, controller.OnPlayerGone);
        socket.On(SocketEvents.PlayerOtherPlayers, controller.OnOtherPlayersReceived);
        socket.On(SocketEvents.PlayerRemoteMove, controller.OnRemotePlayerMovement);

        Debug.Log("Socket configured");
    }

    private SocketIOComponent GetSocket()
    {
        if (socket == null)
            socket = GetComponent<SocketIOComponent>();
        return socket;

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
