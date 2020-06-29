using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Commands
{
    public class RotationCommandPayload
    {
        public Transform TargetTransform { get; set; }
        public Rigidbody TargetRigidbody { get; set; }
        public float HorizontalChange { get; set; }
        public float RotationSpeed { get; set; }
    }

    public interface IRotationCommand
    {
        void Execute(RotationCommandPayload payload);
    }

    /// <summary>
    /// Transform rotation does not work with physics / collisions
    /// </summary>
    public class RigidbodyRotationCommand : IRotationCommand
    {
        private readonly IUnityTimeProxy unityTimeProxy;

        public RigidbodyRotationCommand(IUnityTimeProxy unityTimeProxy)
        {
            this.unityTimeProxy = unityTimeProxy;
        }

        public void Execute(RotationCommandPayload payload)
        {
            var degreesY = payload.HorizontalChange * payload.RotationSpeed * unityTimeProxy.deltaTime;
            var rotationY = Quaternion.Euler(0F, degreesY, 0F);
            payload.TargetRigidbody.MoveRotation(payload.TargetRigidbody.rotation * rotationY);
        }
    }

    /// <summary>
    /// Rigidbody rotation is afefcted by physics (reacts to colliders)
    /// </summary>
    public class TransformRotationCommand : IRotationCommand
    {
        private readonly IUnityTimeProxy unityTimeProxy;

        public TransformRotationCommand(IUnityTimeProxy unityTimeProxy)
        {
            this.unityTimeProxy = unityTimeProxy;
        }

        public void Execute(RotationCommandPayload payload)
        {
            var h = payload.HorizontalChange * payload.RotationSpeed * unityTimeProxy.deltaTime;
            payload.TargetTransform.Rotate(Vector3.up, h);
        }
    }
}
