using Assets.Scripts.Controllers;
using UnityEngine;
using Zenject;

public class LocalMovement : MonoBehaviour
{
    [Inject]
    private ILocalMovementController controller;

    public float force = 7f;
    public float rotationSpeed = 100f;
    public ForceMode forceMode = ForceMode.Force;

    private GameObject localPlayer = null;
    private Vector3 direction = Vector3.zero;

    void Start()
    {
        localPlayer = this.gameObject;
        direction = localPlayer.transform.forward;
        controller.SetLocalPlayer(localPlayer);
        controller.SetSpeed(this.force);
        controller.SetRotationSpeed(this.rotationSpeed);
    }

    void Update()
    {
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
}
