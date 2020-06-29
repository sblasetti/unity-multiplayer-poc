using System;
using System.Collections.Generic;
using ModestTree;
using SocketIO;
using UnityEngine;
using Zenject;

public interface IPlayersManagementController
{
    GameObject GetLocalPlayer();
    GameObject GetRemotePlayer(string id);
    void OnOtherPlayersReceived(SocketIOEvent e);
    void OnPlayerAdded(SocketIOEvent e);
    void OnPlayerGone(SocketIOEvent e);
    void OnPlayerWelcome(SocketIOEvent e);
    void SetState(IGameState state);
    void SetPlayerPrefab(GameObject playerPrefab);
    void SetSocket(ISocketIOComponent socket);
}

public class PlayersManagementController : IPlayersManagementController
{
    private GameObject localPlayer;
    private readonly IUnityGameObjectProxy unityGameObjectProxy;
    private readonly IUnityObjectProxy unityObjectProxy;
    private readonly IUnityDebugProxy unityDebugProxy;
    private readonly IInstantiator container;

    public PlayersManagementController(IUnityGameObjectProxy unityGameObjectProxy, IUnityObjectProxy unityObjectProxy, IUnityDebugProxy unityDebugProxy, IInstantiator container)
    {
        this.unityGameObjectProxy = unityGameObjectProxy;
        this.unityObjectProxy = unityObjectProxy;
        this.unityDebugProxy = unityDebugProxy;
        this.container = container;
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
        Assert.That(e.HasPayload());

        var playerId = e.GetString(SOCKET_DATA_FIELDS.PlayerId);
        if (GetRemotePlayer(playerId) != null) return;

        AddPlayerFromPayload(e.GetPayload());
    }

    public void OnPlayerGone(SocketIOEvent e)
    {
        var playerId = e.GetString(SOCKET_DATA_FIELDS.PlayerId);
        unityDebugProxy.Log($"{e.name} - {playerId}");

        RemoveRemotePlayer(playerId);
    }

    public void OnPlayerWelcome(SocketIOEvent e)
    {
        Debug.Log($"welcomed, current player {(GetLocalPlayer() != null ? "set" : "null")}");

        if (GetLocalPlayer() != null) return;

        Assert.That(e.HasPayload());
        Assert.That(e.HasField("x"));
        Assert.That(e.HasField("y"));

        var x = e.GetFloat("x").Value;
        var y = e.GetFloat("y").Value;

        var position = new Vector3(x, 0, y);
        this.localPlayer = CreatePlayer("local", position, false);

        this.socket.EmitIfConnected(SOCKET_EVENTS.PlayerJoin, new JSONObject());
    }

    public void OnOtherPlayersReceived(SocketIOEvent e)
    {
        var data = e.GetPayload();
        unityDebugProxy.Log($"{e.name} - {data}");

        var players = data.list;
        foreach (var player in players)
        {
            var playerId = player.GetField(SOCKET_DATA_FIELDS.PlayerId).str;
            if (!state.RemotePlayerExists(playerId))
                AddPlayerFromPayload(player);

            unityDebugProxy.Log($"Added remote player {player}");
        }
    }

    private void AddPlayerFromPayload(JSONObject jobj)
    {
        var playerId = jobj.GetField(SOCKET_DATA_FIELDS.PlayerId).str;
        var posObj = jobj.GetField(SOCKET_DATA_FIELDS.Position);
        var posX = posObj.GetField(SOCKET_DATA_FIELDS.PositionX).f;
        var posY = posObj.GetField(SOCKET_DATA_FIELDS.PositionY).f;

        AddRemotePlayer(playerId, posX, posY);
    }

    private GameObject CreatePlayer(string id, Vector3 position, bool isRemote = true)
    {
        var gobj = container.InstantiatePrefab(playerPrefab, position, Quaternion.identity, null);
        gobj.name = $"Player:{id}"; // TODO: improve
        if (!isRemote)
        {
            container.InstantiateComponent<LocalMovement>(gobj);
        }
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
