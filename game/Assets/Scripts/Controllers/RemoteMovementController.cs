using SocketIO;

public interface IRemoteMovementController
{
    void OnRemotePlayerMovement(SocketIOEvent e);
    void SetState(IGameState @object);
}

public class RemoteMovementController : IRemoteMovementController
{
    IGameState state;
    public void SetState(IGameState state)
    {
        this.state = state;
    }
    public void OnRemotePlayerMovement(SocketIOEvent e)
    {
        // var playerId = e.GetString("id");
        // var player = GetRemotePlayer(playerId);

        // var horizontal = e.GetFloat("horizontal");
        // var vertical = e.GetFloat("vertical");

        // unityDebugProxy.Log($"id: {playerId} | h: {horizontal} | v: {vertical}");

        // if (vertical.HasValue && vertical != 0)
        // {
        //     var v = vertical.Value * 10f * Time.deltaTime;
        //     // rb.AddForce(localPlayer.transform.forward * v, forceMode);
        //     // rb.velocity = localPlayer.transform.forward * vertical * force;
        //     var rb = player.GetComponent<Rigidbody>();
        //     rb.MovePosition(player.transform.position + (player.transform.forward * v));
        // }
        // if (horizontal.HasValue && horizontal != 0)
        // {
        //     var h = horizontal.Value * 100f * Time.deltaTime;
        //     player.transform.Rotate(Vector3.up, h);
        // }
    }
}