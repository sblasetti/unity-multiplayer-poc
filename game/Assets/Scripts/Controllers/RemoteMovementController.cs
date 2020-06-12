using SocketIO;
using UnityEngine;

public interface IRemoteMovementController
{
    void OnRemotePlayerMovement(SocketIOEvent e);
    void SetState(IGameState state);
    void SetMovementSpeed(float movementSpeed);
    void SetRotatonSpeed(float rotationSpeed);
}

public class RemoteMovementController : IRemoteMovementController
{
    private IUnityGameObjectProxy unityGameObjectProxy;
    private IUnityTimeProxy unityTimeProxy;

    public RemoteMovementController(IUnityGameObjectProxy unityGameObjectProxy, IUnityTimeProxy unityTimeProxy)
    {
        this.unityGameObjectProxy = unityGameObjectProxy;
        this.unityTimeProxy = unityTimeProxy;
    }

    float movementSpeed;
    public void SetMovementSpeed(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }
    float rotationSpeed;
    public void SetRotatonSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }
    IGameState state;
    public void SetState(IGameState state)
    {
        this.state = state;
    }
    public void OnRemotePlayerMovement(SocketIOEvent e)
    {
        var playerId = e.GetString(SOCKET_DATA_FIELDS.PlayerId);
        var player = unityGameObjectProxy.Find($"Player:{playerId}");

        var horizontal = e.GetFloat(SOCKET_DATA_FIELDS.HorizontalMovement) ?? 0;
        var vertical = e.GetFloat(SOCKET_DATA_FIELDS.VerticalMovement) ?? 0;

        Debug.Log($"player found: {player != null}");

        if (vertical != 0)
        {
            var v = vertical * this.movementSpeed * unityTimeProxy.deltaTime;
            Debug.Log($"{vertical} - {v} - {movementSpeed}");
            // rb.AddForce(localPlayer.transform.forward * v, forceMode);
            // rb.velocity = localPlayer.transform.forward * vertical * force;
            var rb = player.GetComponent<Rigidbody>();
            rb.MovePosition(player.transform.position + (player.transform.forward * v));
        }

        if (horizontal != 0)
        {
            // var h = horizontal * this.rotationSpeed * unityTimeProxy.deltaTime * Mathf.Rad2Deg;
            // player.transform.Rotate(0, h, 0);
            var h = horizontal * this.rotationSpeed * unityTimeProxy.deltaTime;
            player.transform.Rotate(Vector3.up, h);
        }
    }
}