using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Commands
{
    public class MovementCommandPayload {
        public Transform TargetTransform { get; set; }
        public Rigidbody TargetRigidbody { get; set; }
        public float VerticalChange { get; set; }
        public float Speed { get; set; }
    }

    public interface IMovementCommand
    {
        void Execute(MovementCommandPayload payload);
    }

    /// <summary>
    /// Rigidbody movement is afefcted by physics (reacts to colliders)
    /// </summary>
    public class RigidbodyMovementCommand : IMovementCommand
    {
        private readonly IUnityTimeProxy unityTimeProxy;

        public RigidbodyMovementCommand(IUnityTimeProxy unityTimeProxy)
        {
            this.unityTimeProxy = unityTimeProxy;
        }

        public void Execute(MovementCommandPayload payload)
        {
            var distanceForward = payload.VerticalChange * payload.Speed * unityTimeProxy.deltaTime;
            payload.TargetRigidbody.MovePosition(payload.TargetTransform.position + payload.TargetTransform.forward * distanceForward);
        }
    }

    /// <summary>
    /// Transform movement does not work with physics / collisions
    /// </summary>
    public class TransformMovementCommand
    {
        private readonly IUnityTimeProxy unityTimeProxy;

        public TransformMovementCommand(IUnityTimeProxy unityTimeProxy)
        {
            this.unityTimeProxy = unityTimeProxy;
        }

        public void Execute(MovementCommandPayload payload)
        {
            var distance = payload.VerticalChange * payload.Speed * unityTimeProxy.deltaTime;
            payload.TargetTransform.Translate(Vector3.forward * distance);
        }
    }
}
