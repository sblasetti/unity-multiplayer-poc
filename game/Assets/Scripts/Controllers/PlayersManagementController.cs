using System;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class PlayersManagementController
{
    public PlayersManagementController(IUnityObjectProxy unityObjectProxy, IUnityDebugProxy unityDebugProxy)
    {
        this.unityObjectProxy = unityObjectProxy;
        this.unityDebugProxy = unityDebugProxy;
    }

    internal void SetPlayerPrefab(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    GameObject playerPrefab;
    GameObject localPlayer;
    SocketIOComponent socket;
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();
    IUnityObjectProxy unityObjectProxy;
    IUnityDebugProxy unityDebugProxy;

    public GameObject GetLocalPlayer()
    {
        if (localPlayer == null)
            localPlayer = CreatePlayer("local");
        return localPlayer;
    }
    public void OnConnectionOpen(SocketIOEvent e)
    {
        unityDebugProxy.Log("connected");
        socket.Emit("player:data");
    }

    public void OnPlayerAdded(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        unityDebugProxy.Log($"{e.name} - {playerId}");

        AddRemotePlayer(playerId);
    }

    public void OnPlayerGone(SocketIOEvent e)
    {
        var playerId = e.data.GetField("id").str;
        unityDebugProxy.Log($"{e.name} - {playerId}");

        RemoveRemotePlayer(playerId);
    }

    public void OnOtherPlayersReceived(SocketIOEvent e)

    {
        var data = e.data.GetField("players");
        unityDebugProxy.Log($"{e.name} - {data}");

        var players = data.list;
        foreach (var player in players)
        {
            var playerId = player.GetField("id").str;
            AddRemotePlayer(playerId);

            unityDebugProxy.Log($"Added remote player {playerId}");
        }
    }

    private GameObject CreatePlayer(string id)
    {
        var gobj = unityObjectProxy.Instantiate(playerPrefab);
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
            unityObjectProxy.Destroy(player);
            remotePlayers.Remove(playerId);
        }
    }
}
