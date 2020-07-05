using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGameState
{
    void AddRemotePlayer(string playerId, float posX, float posY, float posZ);
    void RemoveRemotePlayer(string playerId);
    bool RemotePlayerExists(string playerId);
}

[CreateAssetMenu(menuName = "Seba/GameSceneState")]
public class GameSceneState : ScriptableObject, IGameState
{
    private readonly List<PlayerData> remotePlayers;

    public GameSceneState()
    {
        remotePlayers = new List<PlayerData>();
    }

    public void AddRemotePlayer(string playerId, float posX, float posY, float posZ)
    {
        PlayerData data = BuildPlayerData(playerId, posX, posY, posZ);
        if (data == null) return;

        remotePlayers.Add(data);
    }

    public void RemoveRemotePlayer(string playerId)
    {
        this.remotePlayers.RemoveAll(x => x.Id == playerId);
    }

    public bool RemotePlayerExists(string playerId)
    {
        return remotePlayers.Any(x => x.Id == playerId);
    }

    private static PlayerData BuildPlayerData(string playerId, float posX, float posY, float posZ)
    {
        var obj = ScriptableObject.CreateInstance(typeof(PlayerData)) as PlayerData;
        if (obj == null) return null;

        obj.name = playerId;
        obj.Id = playerId;
        obj.PositionX = posX;
        obj.PositionY = posY;
        obj.PositionZ = posZ;
        return obj;
    }
}
