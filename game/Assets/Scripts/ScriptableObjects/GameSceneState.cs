using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    void AddRemotePlayer(string playerId, float posX, float posY);
    void RemoveRemotePlayer(string playerId);
}

[CreateAssetMenu(menuName = "Seba/GameSceneState")]
public class GameSceneState : ScriptableObject, IGameState
{
    private List<PlayerData> remotePlayers = new List<PlayerData>();

    public void AddRemotePlayer(string playerId, float posX, float posY)
    {
        PlayerData data = BuildPlayerData(playerId, posX, posY);
        if (data == null) return;

        remotePlayers.Add(data);
    }

    public void RemoveRemotePlayer(string playerId)
    {
        this.remotePlayers.RemoveAll(x => x.Id == playerId);
    }

    private static PlayerData BuildPlayerData(string playerId, float posX, float posY)
    {
        var obj = ScriptableObject.CreateInstance(typeof(PlayerData)) as PlayerData;
        if (obj == null) return null;

        obj.name = playerId;
        obj.Id = playerId;
        obj.PositionX = posX;
        obj.PositionY = posY;
        return obj;
    }
}
