using System;
using System.Collections.Generic;
using ModestTree;
using SocketIO;
using UnityEngine;

public interface IPlayersManagementController
{
    GameObject GetLocalPlayer();
    GameObject GetRemotePlayer(string id);
    void OnOtherPlayersReceived(SocketIOEvent e);
    void OnPlayerAdded(SocketIOEvent e);
    void OnPlayerGone(SocketIOEvent e);
    void OnPlayerInitialPosition(SocketIOEvent e);
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
        return localPlayer;
    }

    public GameObject GetRemotePlayer(string id)
    {
        var name = $"Player:{id}"; // TODO: improve
        return unityGameObjectProxy.Find(name);
    }

    public void OnPlayerAdded(SocketIOEvent e)
    {
        var playerId = e.data.GetField(SOCKET_DATA_FIELDS.PlayerId).str;
        if (GetRemotePlayer(playerId) != null) return;

        AddPlayerFromJSONObject(e.data);
    }

    private void AddPlayerFromJSONObject(JSONObject jobj)
    {
        var playerId = jobj.GetField(SOCKET_DATA_FIELDS.PlayerId).str;
        var posObj = jobj.GetField(SOCKET_DATA_FIELDS.Position);
        var posX = posObj.GetField(SOCKET_DATA_FIELDS.PositionX).f;
        var posY = posObj.GetField(SOCKET_DATA_FIELDS.PositionY).f;

        AddRemotePlayer(playerId, posX, posY);
    }

    public void OnPlayerGone(SocketIOEvent e)
    {
        var playerId = e.data.GetField(SOCKET_DATA_FIELDS.PlayerId).str;
        unityDebugProxy.Log($"{e.name} - {playerId}");

        RemoveRemotePlayer(playerId);
    }

    public void OnPlayerInitialPosition(SocketIOEvent e)
    {
        Debug.Log("position received, player: " + localPlayer);

        if (localPlayer != null) return;

        Assert.That(e.data.HasField("x"));
        Assert.That(e.data.HasField("y"));

        var x = e.GetFloat("x").Value;
        var y = e.GetFloat("y").Value;

        var position = new Vector3(x, 0, y);
        this.localPlayer = this.unityObjectProxy.Instantiate(playerPrefab, position, Quaternion.identity);
    }

    public void SendPlayerMove(float initialX, float initialY, float horizontal, float vertical)
    {
        var data = BuildPositionData(initialX, initialY, horizontal, vertical);
        socket.EmitIfConnected(SOCKET_EVENTS.PlayerLocalMove, data);
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
            AddPlayerFromJSONObject(player);

            unityDebugProxy.Log($"Added remote player {player}");
        }
    }

    private GameObject CreatePlayer(string id, Vector3 position)
    {
        var gobj = unityObjectProxy.Instantiate(playerPrefab, position, Quaternion.identity);
        gobj.name = $"Player:{id}"; // TODO: improve
        return gobj;
    }

    private void AddRemotePlayer(string playerId, float posX, float posY)
    {
        CreatePlayer(playerId, new Vector3(posX, 0, posY));
        this.state.AddRemotePlayer(playerId, posX,  posY);
    }

    private void RemoveRemotePlayer(string playerId)
    {
        this.state.RemoveRemotePlayer(playerId);

        var name = $"Player:{playerId}"; // TODO: improve
        var player = unityGameObjectProxy.Find(name);
        if (player != null)
        {
            unityObjectProxy.DestroyImmediate(player);
        }
    }
}
