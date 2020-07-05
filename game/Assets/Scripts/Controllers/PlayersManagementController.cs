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

        var position = new Vector3(x, 0.5F, y);
        this.localPlayer = CreatePlayer("local", position, Quaternion.identity, false);

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
        string playerId = null;
        jobj.GetField(ref playerId, SOCKET_DATA_FIELDS.PlayerId);
        var position = GetPosition(jobj);
        var rotation = GetRotation(jobj);

        AddRemotePlayer(playerId, position, rotation);
    }

    private static Vector3 GetPosition(JSONObject jobj)
    {
        float x = 0, y = 0, z = 0;
        var aux = jobj.GetField(SOCKET_DATA_FIELDS.Position);
        aux.GetField(ref x, SOCKET_DATA_FIELDS.PositionX);
        aux.GetField(ref y, SOCKET_DATA_FIELDS.PositionY);
        aux.GetField(ref z, SOCKET_DATA_FIELDS.PositionZ);
        return new Vector3(x, y, z);
    }

    private static Quaternion GetRotation(JSONObject jobj)
    {
        float x = 0, y = 0, z = 0, w = 0;
        var aux = jobj.GetField(SOCKET_DATA_FIELDS.Rotation);
        aux.GetField(ref x, SOCKET_DATA_FIELDS.RotationX);
        aux.GetField(ref y, SOCKET_DATA_FIELDS.RotationY);
        aux.GetField(ref z, SOCKET_DATA_FIELDS.RotationZ);
        aux.GetField(ref w, SOCKET_DATA_FIELDS.RotationW);
        return new Quaternion(x, y, z, w);
    }

    private GameObject CreatePlayer(string id, Vector3 position, Quaternion rotation, bool isRemote = true)
    {
        var gobj = container.InstantiatePrefab(playerPrefab, position, rotation, null);
        gobj.name = $"Player:{id}"; // TODO: improve
        if (!isRemote)
        {
            container.InstantiateComponent<LocalMovement>(gobj);
        }
        return gobj;
    }

    private void AddRemotePlayer(string playerId, Vector3 position, Quaternion rotation)
    {
        CreatePlayer(playerId, position, rotation);
        this.state.AddRemotePlayer(playerId, position.x, position.y, position.z);
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
