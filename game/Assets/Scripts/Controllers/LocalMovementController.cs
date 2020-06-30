using Assets.Scripts.Commands;
using Assets.Scripts.Proxies;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public interface ILocalMovementController
    {
        Vector3 GetAxisDirection();
        void PerformLocalMoveOnFixedUpdate(Vector3 direction);
        void SetLocalPlayer(GameObject player);
        void SetSpeed(float speed);
        void SetRotationSpeed(float rotationSpeed);
    }

    public class LocalMovementController : ILocalMovementController
    {
        private readonly IUnityInputProxy unityInputProxy;
        private readonly IUnityPhysicsProxy unityPhysicsProxy;
        private readonly INetworkController networkController;
        private readonly IRotationCommand rotationCommand;
        private readonly IMovementCommand movementCommand;

        private GameObject localPlayer = null;
        private Rigidbody localPlayerRigidbody = null;
        private float groundTreshold = 0.6F;
        private float speed = 0F;
        private float rotationSpeed = 0F;

        public LocalMovementController(IUnityInputProxy unityInputProxy, INetworkController networkController, 
            IRotationCommand rotationCommand, IMovementCommand movementCommand, IUnityPhysicsProxy unityPhysicsProxy)
        {
            this.unityInputProxy = unityInputProxy;
            this.unityPhysicsProxy = unityPhysicsProxy;
            this.networkController = networkController;
            this.rotationCommand = rotationCommand;
            this.movementCommand = movementCommand;
        }

        public Vector3 GetAxisDirection()
        {
            var horizontal = unityInputProxy.GetAxis(INPUT_NAMES.AxisHorizontal);
            var vertical = unityInputProxy.GetAxis(INPUT_NAMES.AxisVertical);
            return new Vector3(horizontal, 0, vertical);
        }

        public void PerformLocalMoveOnFixedUpdate(Vector3 direction)
        {
            var horizontal = direction.x;
            var vertical = direction.z;

            if (vertical != 0)
            {
                var payload = new MovementCommandPayload
                {
                    TargetTransform = localPlayer.transform,
                    TargetRigidbody = localPlayerRigidbody,
                    Speed = speed,
                    VerticalChange = vertical
                };
                movementCommand.Execute(payload);
            }

            if (horizontal != 0 && IsGrounded())
            {
                var payload = new RotationCommandPayload { 
                    TargetTransform = localPlayer.transform,
                    TargetRigidbody = localPlayerRigidbody,
                    RotationSpeed = rotationSpeed,
                    HorizontalChange = horizontal
                };
                rotationCommand.Execute(payload);
            }

            if (localPlayer != null)
                Debug.DrawRay(localPlayer.transform.position, localPlayer.transform.forward, Color.red);

            if (horizontal == 0 && vertical == 0)
                return;

            networkController.SendLocalPositionChange(vertical, horizontal);
        }

        private bool IsGrounded()
        {
            Debug.DrawRay(localPlayer.transform.position, Vector3.down, Color.green);
            var res = unityPhysicsProxy.Raycast(localPlayer.transform.position, Vector3.down, groundTreshold);
            if (!res.Hit) return false;

            var colliderObject = res.ColliderGameObject;
            return colliderObject != null && (colliderObject.GetComponent<IsGround>() != null || colliderObject.GetComponentInParent<IsGround>() != null);
        }

        public void SetLocalPlayer(GameObject player)
        {
            this.localPlayer = player;
            this.localPlayerRigidbody = player.GetComponent<Rigidbody>();
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }
    }

}
