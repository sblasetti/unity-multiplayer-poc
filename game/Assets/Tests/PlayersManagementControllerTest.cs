using NUnit.Framework;
using Zenject;
using Moq;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace Tests
{
    [TestFixture]
    public class PlayersManagementControllerTest : ZenjectUnitTestFixture
    {
        private IPlayersManagementController controller;
        private Mock<IUnityGameObjectProxy> unityGameObjectProxyMock = new Mock<IUnityGameObjectProxy>();
        private Mock<IUnityObjectProxy> unityObjectProxyMock = new Mock<IUnityObjectProxy>();
        private Mock<IUnityDebugProxy> unityDebugProxyMock = new Mock<IUnityDebugProxy>();

        /* TODO: investigate how to initialize the DI container that the test has as property

            Assert.IsNotNull(Container);
            Container.Bind<IUnityObjectProxy>().FromInstance(unityObjectProxyMock.Object);
            Container.Bind<IUnityDebugProxy>().FromInstance(unityDebugProxyMock.Object);
            Container.Bind<IPlayersManagementController>().AsSingle();

            Container.Inject(controller);

        */

        [SetUp]
        public void SetUp()
        {
            unityDebugProxyMock.Reset();
            unityObjectProxyMock.Reset();
            unityGameObjectProxyMock.Reset();
            controller = new PlayersManagementController(
                unityGameObjectProxyMock.Object, unityObjectProxyMock.Object, unityDebugProxyMock.Object);

            unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>())).Returns(ObjectMother.BuildGenericGameObject());
        }

        [Test]
        public void OnConnectionOpen_SendPlayerDataToTheServer()
        {
            // Given
            var socketMock = new Mock<ISocketIOComponent>();
            controller.SetSocket(socketMock.Object);

            // When
            var socketEvent = ObjectMother.BuildSocketIOEvent("test", ObjectMother.BuildEmptyJSONObject());
            controller.OnConnectionOpen(socketEvent);

            // Then
            socketMock.Verify(x => x.Emit(SocketEvents.PlayerData), Times.Once);
        }

        [Test]
        public void OnPlayerAdded_PlayerIsCreatedLocallyAndAddedToTheListOfRemotePlayers()
        {
            // Given
            AssertPlayersCountIs(0);
            var socketEvent = BuildSocketIOEventWithPlayerId("test", "TEST_ID");

            // When
            controller.OnPlayerAdded(socketEvent);

            // Then: remote player created as local GameObject
            unityObjectProxyMock.Verify(x => x.Instantiate(It.IsAny<GameObject>()), Times.Once);
            // Then: player added to the list of remotes
            AssertPlayersCountIs(1);
            AssertRemotePlayerExists("TEST_ID");
            unityDebugProxyMock.Verify(x => x.Log(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void OnPlayerGone_RemoveRemotePlayerFromLocalGame()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayerId("test", "TEST_ID");
            controller.OnPlayerAdded(socketEvent);
            unityGameObjectProxyMock.Reset();
            var fakeGameObject = ObjectMother.BuildGenericGameObject();
            unityGameObjectProxyMock.Setup(x => x.Find(It.IsAny<string>())).Returns(fakeGameObject);
            AssertPlayersCountIs(1);

            // When
            controller.OnPlayerGone(socketEvent);

            // Then: remote player object removed from game
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Once);
            // Then: player removed from the list of remotes
            AssertPlayersCountIs(0);
            AssertRemotePlayerDoesNotExist("TEST_ID");
        }

        private static SocketIOEvent BuildSocketIOEventWithPlayerId(string eventName, string playerId)
        {
            var jobj = ObjectMother.BuildEmptyJSONObject();
            jobj.AddField("id", playerId);
            var socketEvent = ObjectMother.BuildSocketIOEvent(eventName, jobj);
            return socketEvent;
        }

        private void AssertRemotePlayerExists(string id)
        {
            var player = controller.GetRemotePlayer(id);
            Assert.IsNotNull(player);
        }

        private void AssertRemotePlayerDoesNotExist(string id)
        {
            var player = controller.GetRemotePlayer(id);
            Assert.IsNull(player);
        }

        private void AssertPlayersCountIs(int expected)
        {
            Assert.AreEqual(expected, controller.PlayersCount);
        }
    }

}
