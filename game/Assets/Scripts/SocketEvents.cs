using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SocketEvents
{
    public static string PlayerNew = "player:new";
    public static string PlayerGone = "player:gone";
    public static string PlayerOtherPlayers = "player:other-players";
    public static string PlayerRemoteMove = "player:remote-movement";
    public static string PlayerLocalMove = "player:local-movement";
    public static string SocketOpen = "open";
}
