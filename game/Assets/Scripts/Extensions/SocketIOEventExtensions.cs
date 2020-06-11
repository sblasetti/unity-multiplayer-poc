using SocketIO;

public static class SocketIOEventExtensions
{
    public static string GetString(this SocketIOEvent e, string field)
    {
        return e.data.HasField(field) ? e.data.GetField(field).str : null;
    }

    public static float? GetFloat(this SocketIOEvent e, string field)
    {
        return e.data.HasField(field) ? float.Parse(e.data.GetField(field).str) : (float?)null;
    }
}