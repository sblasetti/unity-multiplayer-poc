using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{

}

public class GameState : IGameState
{
    List<Player> players = new List<Player>();
}

class Player
{
    public bool IsRemote { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
}