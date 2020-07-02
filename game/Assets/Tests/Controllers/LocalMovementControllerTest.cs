using Assets.Scripts.Commands;
using Assets.Scripts.Controllers;
using Assets.Scripts.Proxies;
using Game.Tests.Builders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Tests.Controllers
{
    [TestFixture]
    public class LocalMovementControllerTest
    {
        [Datapoints]
        private float[] directionChange = new float[] { -0.123F, 0F, 0.123F };

        private delegate void RaycastDelegate(Vector3 v1, Vector3 v2, out RaycastHit h, float f);

        private ILocalMovementController controller;

        private Mock<IUnityInputProxy> unityInputProxyMock = new Mock<IUnityInputProxy>();
        private Mock<IUnityPhysicsProxy> unityPhysicsProxyMock = new Mock<IUnityPhysicsProxy>();
        private Mock<INetworkController> networkControllerMock = new Mock<INetworkController>();
        private Mock<IRotationCommand> rotationCommandMock = new Mock<IRotationCommand>();
        private Mock<IMovementCommand> movementCommandMock = new Mock<IMovementCommand>();

        private GameObject fakeLocalPlayer;
        private Rigidbody fakeLocalRigidbody;
        private GameObject fakeGround;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            fakeLocalPlayer = GameObjectBuilder.New()
                .WithRigidbody()
                .WithLocalMovement()
                .Build();
            fakeLocalRigidbody = fakeLocalPlayer.GetComponent<Rigidbody>();

            fakeGround = GameObjectBuilder.New()
                .WithIsGround()
                .Build();
        }

        [SetUp]
        public void SetUp()
        {
            networkControllerMock.Reset();
            unityInputProxyMock.Reset();
            unityPhysicsProxyMock.Reset();
            movementCommandMock.Reset();
            rotationCommandMock.Reset();

            controller = new LocalMovementController(unityInputProxyMock.Object, networkControllerMock.Object, 
                rotationCommandMock.Object, movementCommandMock.Object, unityPhysicsProxyMock.Object);
            controller.SetLocalPlayer(fakeLocalPlayer);
            controller.SetSpeed(30F);
            controller.SetRotationSpeed(40F);
        }

        [Test]
        public void GetAxisDirection_ShouldGetChangesOnBothHorizontalAndVerticalAxis()
        {
            // Given
            unityInputProxyMock.Setup(x => x.GetAxis(INPUT_NAMES.AxisHorizontal)).Returns(1.23F);
            unityInputProxyMock.Setup(x => x.GetAxis(INPUT_NAMES.AxisVertical)).Returns(4.56F);

            // When
            var direction = controller.GetAxisDirection();

            // Then
            Assert.AreEqual(1.23F, direction.x); // horizontal
            Assert.AreEqual(0, direction.y);
            Assert.AreEqual(4.56F, direction.z); // vertical
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_OnVerticalChange(float change)
        {
            // Given
            var fakeDirection = new Vector3(0, 0, change);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            var times = change != 0 ? Times.Once() : Times.Never();
            movementCommandMock.Verify(x => x.Execute(It.IsAny<MovementCommandPayload>()), times);
            unityPhysicsProxyMock.Verify(x => x.Raycast(It.IsAny<Vector3>(), It.IsAny<Vector3>(), It.IsAny<float>()), Times.Never);
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_OnHorizontalChange(float change)
        {
            // Given
            GivenRaycastCollidesWithObject();
            var fakeDirection = new Vector3(change, 0, 0);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            var times = change != 0 ? Times.Once() : Times.Never();
            rotationCommandMock.Verify(x => x.Execute(It.IsAny<RotationCommandPayload>()), times);
            unityPhysicsProxyMock.Verify(x => x.Raycast(It.IsAny<Vector3>(), It.IsAny<Vector3>(), It.IsAny<float>()), times);
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_OnlyRotateWhenTouchingTheGround(bool grounded)
        {
            // Given
            GivenRaycastCollidesWithObject(grounded);
            var fakeDirection = new Vector3(0.1F, 0, 0);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            var times = grounded ? Times.Once() : Times.Never();
            rotationCommandMock.Verify(x => x.Execute(It.IsAny<RotationCommandPayload>()), times);
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_SendNewPositionForValidation()
        {
            // Given
            GivenRaycastCollidesWithObject();
            var fakeDirection = new Vector3(0.1F, 0, 0.3F);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            movementCommandMock.Verify(x => x.Execute(It.IsAny<MovementCommandPayload>()), Times.Once);
            rotationCommandMock.Verify(x => x.Execute(It.IsAny<RotationCommandPayload>()), Times.Once);
            networkControllerMock.Verify(x => x.SendLocalPositionChange(fakeDirection.z, fakeDirection.x), Times.Once);
        }

        private void GivenRaycastCollidesWithObject(bool hit = true)
        {
            unityPhysicsProxyMock.Setup(x => x.Raycast(It.IsAny<Vector3>(), It.IsAny<Vector3>(), It.IsAny<float>()))
                .Returns(new RaycastResult
                {
                    Hit = hit,
                    ColliderGameObject = hit ? fakeGround : null
                });
        }
    }
}
