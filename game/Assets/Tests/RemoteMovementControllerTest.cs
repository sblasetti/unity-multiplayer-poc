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
        // private GameObject fakePlayerPrefab = new GameObject();
        // private Mock<IUnityGameObjectProxy> unityGameObjectProxyMock = new Mock<IUnityGameObjectProxy>();
        // private Mock<IUnityObjectProxy> unityObjectProxyMock = new Mock<IUnityObjectProxy>();
        // private Mock<IUnityDebugProxy> unityDebugProxyMock = new Mock<IUnityDebugProxy>();
        private Mock<IGameState> gameStateMock = new Mock<IGameState>();

        [SetUp]
        public void SetUp()
        {
            // unityDebugProxyMock.Reset();
            // unityObjectProxyMock.Reset();
            // unityGameObjectProxyMock.Reset();
            gameStateMock.Reset();

            controller = new RemoteMovementController();
            // controller.SetPlayerPrefab(fakePlayerPrefab);
            controller.SetState(gameStateMock.Object);

            // unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>())).Returns(ObjectMother.BuildGenericGameObject());
        }

        [Test]
        public void OnRemotePlayerMovement_MovePlayerGameObject()
        {
            // Given
            var jobj = JSONObjectBuilder.Dictionary()
                .Build();

            // When
            var socketEvent = ObjectMother.BuildSocketIOEvent("test", jobj);
            controller.OnRemotePlayerMovement(socketEvent);

            // Then
            Assert.AreEqual(1, 2);
        }
    }
}
