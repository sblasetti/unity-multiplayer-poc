using UnityEngine;
using SocketIO;

public class PlayersManagement : MonoBehaviour
{
    SocketIOComponent socket;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        socket = GetSocket();

        socket.On("open", OnConnectionOpen);
        socket.On("player:new", OnPlayerAdded);
        socket.On("player:gone", OnPlayerGone);

        Debug.Log("Socket configured");
    }

    private SocketIOComponent GetSocket()
    {
        if (socket == null)
            socket = GetComponent<SocketIOComponent>();

        return socket;
    }

    void OnConnectionOpen(SocketIOEvent e)
    {
        Debug.Log("connected");
        socket.Emit("player:data");
    }

    void OnPlayerAdded(SocketIOEvent e)
    {
        Debug.Log($"{e.name} - {e.data.GetField("id")}");
        Instantiate(playerPrefab);
    }

    void OnPlayerGone(SocketIOEvent e)
    {
        Debug.Log($"{e.name} - {e.data.GetField("id")}");
    }
}
