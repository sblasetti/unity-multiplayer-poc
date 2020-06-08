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
    void OnRemotePlayerMovement(SocketIOEvent e);
    void SetPlayerPrefab(GameObject playerPrefab);
    void SetSocket(SocketIOComponent socket);
    void SendPlayerMove(float initialX, float initialY, float horizontal, float vertical);
}

public class PlayersManagementController : IPlayersManagementController
{
    public PlayersManagementController(IUnityObjectProxy unityObjectProxy, IUnityDebugProxy unityDebugProxy)
    {
        this.unityObjectProxy = unityObjectProxy;
        this.unityDebugProxy = unityDebugProxy;
    }

    GameObject playerPrefab;
    public void SetPlayerPrefab(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    SocketIOComponent socket;
    public void SetSocket(SocketIOComponent socket)
    {
        this.socket = socket;
    }

    GameObject localPlayer;
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();
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
        if (!remotePlayers.ContainsKey(id))
            return null; // TODO: error?

        return remotePlayers[id];
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

    public void OnRemotePlayerMovement(SocketIOEvent e)
    {
        var playerId = GetString(e, "id");
        var player = GetRemotePlayer(playerId);

        var horizontal = GetFloat(e, "horizontal");
        var vertical = GetFloat(e, "vertical");

        unityDebugProxy.Log($"id: {playerId} | h: {horizontal} | v: {vertical}");

        if (vertical.HasValue && vertical != 0)
        {
            var v = vertical.Value * 10f * Time.deltaTime;
            // rb.AddForce(localPlayer.transform.forward * v, forceMode);
            // rb.velocity = localPlayer.transform.forward * vertical * force;
            var rb = player.GetComponent<Rigidbody>();
            rb.MovePosition(player.transform.position + (player.transform.forward * v));
        }
        if (horizontal.HasValue && horizontal != 0)
        {
            var h = horizontal.Value * 100f * Time.deltaTime;
            player.transform.Rotate(Vector3.up, h);
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

    private string GetString(SocketIO.SocketIOEvent e, string field)
    {
        return e.data.HasField(field) ? e.data.GetField(field).str : null;
    }

    private float? GetFloat(SocketIO.SocketIOEvent e, string field)
    {
        return e.data.HasField(field) ? float.Parse(e.data.GetField(field).str) : (float?)null;
    }
}
