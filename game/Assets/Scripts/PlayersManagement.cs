using UnityEngine;
using SocketIO;

public class PlayersManagement : MonoBehaviour
{
    SocketIOComponent socket;
    PlayersManagementController controller = new PlayersManagementController(new UnityProxy());

    // Start is called before the first frame update
    void Start()
    {
        SetupSocketEventListeners();
    }

    private void SetupSocketEventListeners()
    {
        socket = GetSocket();

        socket.On(SocketEvents.SocketOpen, controller.OnConnectionOpen);
        socket.On(SocketEvents.PlayerNew, controller.OnPlayerAdded);
        socket.On(SocketEvents.PlayerGone, controller.OnPlayerGone);
        socket.On(SocketEvents.PlayerOtherPlayers, controller.OnOtherPlayersReceived);

        Debug.Log("Socket configured");
    }

    private SocketIOComponent GetSocket()
    {
        if (socket == null)
            socket = GetComponent<SocketIOComponent>();

        return socket;
    }
}
