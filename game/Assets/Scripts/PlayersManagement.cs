using UnityEngine;
using SocketIO;
using System;
using System.Collections.Generic;

public class PlayersManagement : MonoBehaviour
{
    SocketIOComponent socket;
    public GameObject playerPrefab;
    GameObject localPlayer;
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        socket = GetSocket();

        socket.On(SocketEvents.SocketOpen, OnConnectionOpen);
        socket.On(SocketEvents.PlayerNew, OnPlayerAdded);
        socket.On(SocketEvents.PlayerGone, OnPlayerGone);
        socket.On(SocketEvents.PlayerOtherPlayers, OnOtherPlayersReceived);

        Debug.Log("Socket configured");
    }

    internal GameObject GetLocalPlayer()
    {
        if (localPlayer == null)
            localPlayer = CreatePlayer("local");

        return localPlayer;
    }

    private GameObject CreatePlayer(string id)
    {
        var gobj = Instantiate(playerPrefab);
        gobj.name = $"Player:{id}"; // TODO: improve
        return gobj;
    }

    private SocketIOComponent GetSocket()
    {
        if (socket == null)
            socket = GetComponent<SocketIOComponent>();

        return socket;
    }

    void OnConnectionOpen(SocketIOEvent e)
    {
        Debug.Log("connected");
        socket.Emit("player:data");
    }

    void OnPlayerAdded(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        Debug.Log($"{e.name} - {playerId}");
        var player = CreatePlayer(playerId);
        remotePlayers.Add(playerId, player);
    }

    void OnPlayerGone(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        Debug.Log($"{e.name} - {playerId}");

        var name = $"Player:{playerId}"; // TODO: improve
        var player = GameObject.Find(name);
        if (player != null)
        {
            Destroy(player);
            remotePlayers.Remove(playerId);
        }
    }

    void OnOtherPlayersReceived(SocketIOEvent e)
    {
        var data = e.data.GetField("players");
        Debug.Log($"{e.name} - {data}");

        var players = data.list;
        foreach (var player in players)
        {
            var playerId = e.data.GetField("id").str;
            var playerObject = CreatePlayer(playerId);
            remotePlayers.Add(playerId, playerObject);
            Debug.Log($"Added remote player {playerId}");
        }
    }
}
