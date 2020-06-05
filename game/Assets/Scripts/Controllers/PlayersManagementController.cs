using System;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class PlayersManagementController
{
    public PlayersManagementController(IUnityProxy unityProxy)
    {
        this.unityProxy = unityProxy;
    }
    public GameObject playerPrefab;
    GameObject localPlayer;
    SocketIOComponent socket;
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();
    IUnityProxy unityProxy;
    private GameObject GetLocalPlayer()
    {
        if (localPlayer == null)
            localPlayer = CreatePlayer("local");

        return localPlayer;
    }

    private GameObject CreatePlayer(string id)
    {
        var gobj = unityProxy.Instantiate(playerPrefab);
        gobj.name = $"Player:{id}"; // TODO: improve
        return gobj;
    }

    private void AddRemotePlayer(string playerId)
    {
        var player = CreatePlayer(playerId);
        remotePlayers.Add(playerId, player);
    }

    private void RemoveRemotePlayer(string playerId)
    {
        var name = $"Player:{playerId}"; // TODO: improve
        var player = GameObject.Find(name);
        if (player != null)
        {
            unityProxy.Destroy(player);
            remotePlayers.Remove(playerId);
        }
    }

    public void OnConnectionOpen(SocketIOEvent e)
    {
        Debug.Log("connected");
        socket.Emit("player:data");
    }

    public void OnPlayerAdded(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        Debug.Log($"{e.name} - {playerId}");

        controller.AddRemotePlayer(playerId);
    }

    public void OnPlayerGone(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        Debug.Log($"{e.name} - {playerId}");

        controller.RemoveRemotePlayer(playerId);
    }

    public void OnOtherPlayersReceived(SocketIOEvent e)
    {
        var data = e.data.GetField("players");
        Debug.Log($"{e.name} - {data}");

        var players = data.list;
        foreach (var player in players)
        {
            var playerId = player.GetField("id").str;
            controller.AddRemotePlayer(playerId);

            Debug.Log($"Added remote player {playerId}");
        }
    }
}
