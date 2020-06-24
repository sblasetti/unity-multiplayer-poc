using NUnit.Framework;
using Zenject;
using Moq;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace Tests
{
    [TestFixture]
    public class RemoteMovementControllerTest : ZenjectUnitTestFixture
    {
        private IRemoteMovementController controller;
        private GameObject fakePlayerInstance;
        private Mock<IUnityGameObjectProxy> unityGameObjectProxyMock = new Mock<IUnityGameObjectProxy>();
        private Mock<IUnityTimeProxy> unityTimeProxyMock = new Mock<IUnityTimeProxy>();
        private Mock<IGameState> gameStateMock = new Mock<IGameState>();

        [SetUp]
        public void SetUp()
        {
            unityGameObjectProxyMock.Reset();
            unityTimeProxyMock.Reset();
            gameStateMock.Reset();
            fakePlayerInstance = GameObjectBuilder.New()
                .WithPositionAndRotation(Vector3.zero, Quaternion.identity)
                .WithRigidbody()
                .Build();

            controller = new RemoteMovementController(unityGameObjectProxyMock.Object, unityTimeProxyMock.Object);
            controller.SetState(gameStateMock.Object);

            unityGameObjectProxyMock.Setup(x => x.Find(It.IsAny<string>())).Returns(fakePlayerInstance);
        }

        [Test]
        public void OnRemotePlayerMovement_MovePlayerGameObject()
        {
            // Given
            const float horizontal = 1f;
            const float vertical = 2f;
            var jobj = JSONObjectBuilder.Dictionary()
                .WithPlayerId("TEST_ID")
                .WithMovementCoordinates(horizontal, vertical)
                .Build();
            var socketEvent = SocketIOEventBuilder.New("test").WithData(jobj).Build();
            const float time = 0.123f;
            unityTimeProxyMock.Setup(x => x.deltaTime).Returns(time);
            const float movementSpeed = 12f;
            const float rotationSpeed = 7f;
            controller.SetMovementSpeed(movementSpeed);
            controller.SetRotatonSpeed(rotationSpeed);

            // When
            Debug.Log(fakePlayerInstance.transform.position);
            controller.OnRemotePlayerMovement(socketEvent);
            Debug.Log(fakePlayerInstance.transform.position);

            // Then
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Once);
            const float rotationAngle = horizontal * rotationSpeed * time * Mathf.Rad2Deg;
            Assert.AreEqual(vertical * movementSpeed * time, fakePlayerInstance.transform.position.x);
            Assert.AreEqual(rotationAngle, fakePlayerInstance.transform.rotation.y);
        }
    }
}
