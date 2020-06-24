using SocketIO;

public static class SocketIOExtensions{
    public static string GetString(this SocketIOEvent e, string field)
    {
        return e.data.HasField(field) ? e.data.GetField(field).str : null;
    }

    public static float? GetFloat(this SocketIOEvent e, string field)
    {
        if (!e.data.HasField(field)) return null;

        var jobj = e.data.GetField(field);
        return jobj.IsNumber ? jobj.n : float.Parse(jobj.str);
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