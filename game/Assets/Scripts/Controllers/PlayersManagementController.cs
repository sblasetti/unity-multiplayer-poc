using System;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public interface IPlayersManagementController
{
    GameObject GetLocalPlayer();
    GameObject GetRemotePlayer(string id);
    void OnConnectionOpen(SocketIOEvent e);
    void OnOtherPlayersReceived(SocketIOEvent e);
    void OnPlayerAdded(SocketIOEvent e);
    void OnPlayerGone(SocketIOEvent e);
    void SetState(IGameState state);
    void SetPlayerPrefab(GameObject playerPrefab);
    void SetSocket(ISocketIOComponent socket);
    void SendPlayerMove(float initialX, float initialY, float horizontal, float vertical);
}

public class PlayersManagementController : IPlayersManagementController
{
    public PlayersManagementController(IUnityGameObjectProxy unityGameObjectProxy, IUnityObjectProxy unityObjectProxy, IUnityDebugProxy unityDebugProxy)
    {
        this.unityGameObjectProxy = unityGameObjectProxy;
        this.unityObjectProxy = unityObjectProxy;
        this.unityDebugProxy = unityDebugProxy;
    }

    #region Unity objects

    GameObject playerPrefab;
    public void SetPlayerPrefab(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    ISocketIOComponent socket;
    public void SetSocket(ISocketIOComponent socket)
    {
        this.socket = socket;
    }

    IGameState state;
    public void SetState(IGameState state)
    {
        this.state = state;
    }

    #endregion

    GameObject localPlayer;
    IUnityGameObjectProxy unityGameObjectProxy;
    IUnityObjectProxy unityObjectProxy;
    IUnityDebugProxy unityDebugProxy;

    public GameObject GetLocalPlayer()
    {
        if (localPlayer == null)
            localPlayer = CreatePlayer("local");
        return localPlayer;
    }
    public GameObject GetRemotePlayer(string id)
    {
        var name = $"Player:{id}"; // TODO: improve
        return unityGameObjectProxy.Find(name);
    }

    public void OnConnectionOpen(SocketIOEvent e)
    {
        unityDebugProxy.Log("connected");
        socket.Emit(SocketEvents.PlayerData);
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

    public void SendPlayerMove(float initialX, float initialY, float horizontal, float vertical)
    {
        var data = BuildPositionData(initialX, initialY, horizontal, vertical);
        socket.Emit(SocketEvents.PlayerLocalMove, data);
    }

    private static JSONObject BuildPositionData(float initialX, float initialY, float horizontal, float vertical)
    {
        // TODO: immprove
        var dict = new Dictionary<string, string>() {
            {"initialX", horizontal.ToString()},
            {"initialY", horizontal.ToString()},
            {"horizontalMovement", horizontal.ToString()},
            {"verticalMovement", vertical.ToString()},
        };
        return new JSONObject(dict);
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
        CreatePlayer(playerId);
        this.state.AddRemotePlayer(playerId);
    }

    private void RemoveRemotePlayer(string playerId)
    {
        this.state.RemoveRemotePlayer(playerId);

        var name = $"Player:{playerId}"; // TODO: improve
        var player = unityGameObjectProxy.Find(name);
        if (player != null)
        {
            unityObjectProxy.Destroy(player);
        }
    }
}
