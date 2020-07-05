using Assets.Scripts.Commands;
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
    private readonly IUnityGameObjectProxy unityGameObjectProxy;

    public RemoteMovementController(IUnityGameObjectProxy unityGameObjectProxy)
    {
        this.unityGameObjectProxy = unityGameObjectProxy;
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
        var newPosition = GetPosition(e);
        var rotation = GetRotation(e);

        var playerId = e.GetString(SOCKET_DATA_FIELDS.PlayerId);
        var player = unityGameObjectProxy.Find($"Player:{playerId}");
        var playerRigidbody = player.GetComponent<Rigidbody>();
        playerRigidbody.MoveRotation(rotation);
        playerRigidbody.MovePosition(newPosition);
    }

    private static Vector3 GetPosition(SocketIOEvent e)
    {
        var aux = e.GetField(SOCKET_DATA_FIELDS.Position);
        var newPosition = new Vector3();

        aux.GetField(ref newPosition.x, SOCKET_DATA_FIELDS.PositionX);
        aux.GetField(ref newPosition.y, SOCKET_DATA_FIELDS.PositionY);
        aux.GetField(ref newPosition.z, SOCKET_DATA_FIELDS.PositionZ);

        return newPosition;
    }

    private static Quaternion GetRotation(SocketIOEvent e)
    {
        var aux = e.GetField(SOCKET_DATA_FIELDS.Rotation);
        var rotation = new Quaternion();

        aux.GetField(ref rotation.x, SOCKET_DATA_FIELDS.RotationX);
        aux.GetField(ref rotation.y, SOCKET_DATA_FIELDS.RotationY);
        aux.GetField(ref rotation.z, SOCKET_DATA_FIELDS.RotationZ);
        aux.GetField(ref rotation.w, SOCKET_DATA_FIELDS.RotationW);

        return rotation;
    }
}