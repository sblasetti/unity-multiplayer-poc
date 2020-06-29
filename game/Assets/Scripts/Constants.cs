public static class SOCKET_EVENTS
{
    public static string PlayerNew = "player:new";
    public static string PlayerJoin = "player:join";
    public static string PlayerGone = "player:gone";
    public static string PlayerOtherPlayers = "player:other-players";
    public static string PlayerRemoteMove = "player:remote-movement";
    public static string PlayerWelcome = "player:welcome";
    public static string PlayerLocalMove = "player:local-movement";
    public static string SocketOpen = "open";
}

public static class SOCKET_DATA_FIELDS
{
    public static string Payload = "payload";
    public static string PlayerId = "id";
    public static string HorizontalMovement = "horizontal";
    public static string VerticalMovement = "vertical";
    public static string Position = "position";
    public static string PositionX = "x";
    public static string PositionY = "y";
}

public static class INPUT_NAMES
{
    public static string AxisHorizontal = "Horizontal";
    public static string AxisVertical = "Vertical";
}