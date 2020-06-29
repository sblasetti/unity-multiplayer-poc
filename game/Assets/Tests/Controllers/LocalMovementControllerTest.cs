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

        private GameObject fakeLocalPlayer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            fakeLocalPlayer = GameObjectBuilder.New()
                .WithRigidbody()
                .Build();
            //fakeDirection = new Vector3(1.22F, 3.44F, 5.66F);
        }

        [SetUp]
        public void SetUp()
        {
            networkControllerMock.Reset();
            unityInputProxyMock.Reset();
            //unityObjectProxyMock.Reset();
            //unityGameObjectProxyMock.Reset();
            //gameStateMock.Reset();

            //fakePlayerPrefab = GameObjectBuilder.New().Build();

            controller = new LocalMovementController(unityInputProxyMock.Object, networkControllerMock.Object);
            controller.SetLocalPlayer(fakeLocalPlayer);
            //controller.SetState(gameStateMock.Object);

            //socketMock = new Mock<ISocketIOComponent>();
            //controller.SetSocket(socketMock.Object);

            //socketMock.Setup(x => x.IsConnected).Returns(true);
            //unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>(), It.IsAny<Vector3>(), It.IsAny<Quaternion>())).Returns(fakeLocalPlayer);
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
            var initialX = fakeLocalPlayer.transform.position.x;
            var initialY = fakeLocalPlayer.transform.position.y;
            var initialZ = fakeLocalPlayer.transform.position.z;
            var fakeDirection = new Vector3(0.987F, 0, change);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            Assert.AreEqual(initialY, fakeLocalPlayer.transform.position.y);
            Assert.AreEqual(initialZ, fakeLocalPlayer.transform.position.z);
            if (change != 0)
                Assert.That(initialX != fakeLocalPlayer.transform.position.x + change);
            else
                Assert.That(initialX == fakeLocalPlayer.transform.position.x);
        }

        [Theory]
        public void PerformLocalMoveOnFixedUpdate_OnHorizontalChange(float change)
        {
            // Given
            var initialX = fakeLocalPlayer.transform.rotation.x;
            var initialY = fakeLocalPlayer.transform.rotation.y;
            var initialZ = fakeLocalPlayer.transform.rotation.z;
            var initialW = fakeLocalPlayer.transform.rotation.w;
            var fakeDirection = new Vector3(change, 0, 0.987F);

            // When
            controller.PerformLocalMoveOnFixedUpdate(fakeDirection);

            // Then
            Assert.AreEqual(initialY, fakeLocalPlayer.transform.rotation.y);
            Assert.AreEqual(initialZ, fakeLocalPlayer.transform.rotation.z);
            Assert.AreEqual(initialW, fakeLocalPlayer.transform.rotation.w);
            if (change != 0)
                Assert.That(initialX != fakeLocalPlayer.transform.rotation.x + change);
            else
                Assert.That(initialX == fakeLocalPlayer.transform.rotation.x);
        }
    }
}
