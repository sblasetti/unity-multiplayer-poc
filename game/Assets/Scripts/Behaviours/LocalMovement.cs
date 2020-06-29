using Assets.Scripts.Controllers;
using UnityEngine;
using Zenject;

public class LocalMovement : MonoBehaviour
{
    [Inject]
    private ILocalMovementController controller;
    [Inject]
    private IPlayersManagementController playersManagementController;

    public float force = 50f;
    public float rotationSpeed = 100f;
    public ForceMode forceMode = ForceMode.Force;

    private GameObject localPlayer = null;
    private Vector3 direction = Vector3.zero;

    void Awake()
    {
        controller.SetSpeed(this.force);
        controller.SetRotationSpeed(this.rotationSpeed);
    }

    void Update()
    {
        SetLocalPlayerReference();

        direction = controller.GetAxisDirection();

        // TODO: this is a temp way to reset player's position, remove
        if (Input.GetKeyUp(KeyCode.R))
        {
            localPlayer.transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
        }
    }

    void FixedUpdate()
    {
        controller.PerformLocalMoveOnFixedUpdate(direction);
    }

    /// <summary>
    /// As the player is instantiated after a socket event, this assignment needs to be 
    /// done on update.
    /// </summary>
    private void SetLocalPlayerReference()
    {
        if (localPlayer == null)
        {
            localPlayer = playersManagementController.GetLocalPlayer();
            if (localPlayer != null)
            {
                controller.SetLocalPlayer(localPlayer);
                direction = localPlayer.transform.forward;
            }
        }
    }

    //public void SendPlayerMove(float initialX, float initialY, float horizontal, float vertical)
    //{
    //    var data = BuildPositionData(initialX, initialY, horizontal, vertical);
    //    socket.EmitIfConnected(SOCKET_EVENTS.PlayerLocalMove, data);
    //}

    //private static JSONObject BuildPositionData(float initialX, float initialY, float horizontal, float vertical)
    //{
    //    // TODO: immprove
    //    var dict = new Dictionary<string, string>() {
    //        {"initialX", horizontal.ToString()},
    //        {"initialY", horizontal.ToString()},
    //        {"horizontalMovement", horizontal.ToString()},
    //        {"verticalMovement", vertical.ToString()},
    //    };
    //    return new JSONObject(dict);
    //}
}
