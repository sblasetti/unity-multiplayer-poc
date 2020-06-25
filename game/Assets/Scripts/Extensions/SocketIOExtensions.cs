using SocketIO;

public static class SocketIOExtensions{
    public static JSONObject GetField(this SocketIOEvent e, string field)
    {
        if (!e.HasPayload()) return null;
        var payload = e.GetPayload();

        return payload.HasField(field) ? payload.GetField(field) : null;
    }

    public static string GetString(this SocketIOEvent e, string field)
    {
        if (!e.HasPayload()) return null;
        var payload = e.GetPayload();

        return payload.HasField(field) ? payload.GetField(field).str : null;
    }

    public static float? GetFloat(this SocketIOEvent e, string field)
    {
        if (!e.HasPayload()) return null;
        var payload = e.GetPayload();

        if (!payload.HasField(field)) return null;

        var jobj = payload.GetField(field);
        return jobj.IsNumber ? jobj.n : float.Parse(jobj.str);
    }

    public static JSONObject GetPayload(this SocketIOEvent e)
    {
        return e.data.GetField(SOCKET_DATA_FIELDS.Payload);
    }

    public static bool HasPayload(this SocketIOEvent e)
    {
        return e.data.HasField(SOCKET_DATA_FIELDS.Payload);
    }

    public static bool HasField(this SocketIOEvent e, string field)
    {
        return e.HasPayload() && e.GetPayload().HasField(field);
    }

    public static void EmitIfConnected(this ISocketIOComponent socket, string ev, JSONObject data)
    {
        if (socket.IsConnected) 
            socket.Emit(ev, data);
    }

    public static void EmitIfConnected(this ISocketIOComponent socket, string ev)
    {
        if (socket.IsConnected)
            socket.Emit(ev);
    }
}