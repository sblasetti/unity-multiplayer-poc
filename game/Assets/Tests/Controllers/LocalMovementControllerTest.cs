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

        private ILocalMovementController controller;

        private Mock<IUnityInputProxy> unityInputProxyMock = new Mock<IUnityInputProxy>();
        private Mock<INetworkController> networkControllerMock = new Mock<INetworkController>();
        private Mock<IRotationCommand> rotationCommandMock = new Mock<IRotationCommand>();
        private Mock<IMovementCommand> movementCommandMock = new Mock<IMovementCommand>();

        private GameObject fakeLocalPlayer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            fakeLocalPlayer = GameObjectBuilder.New()
                .WithRigidbody()
                .WithLocalMovement()
                .Build();
        }

        [SetUp]
        public void SetUp()
        {
            networkControllerMock.Reset();
            unityInputProxyMock.Reset();
            movementCommandMock.Reset();
            rotationCommandMock.Reset();

            controller = new LocalMovementController(unityInputProxyMock.Object, networkControllerMock.Object, 
                rotationCommandMock.Object, movementCommandMock.Object);
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

        [Test]
        public void PerformLocalMoveOnFixedUpdate_OnlyMoveWhenTouchingTheGround()
        {
            // TBD
            Assert.Fail();
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
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_OnHorizontalChange(float change)
        {
            // Given
            var fakeDirection = new Vector3(change, 0, 0);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            var times = change != 0 ? Times.Once() : Times.Never();
            rotationCommandMock.Verify(x => x.Execute(It.IsAny<RotationCommandPayload>()), times);
        }
    }
}
