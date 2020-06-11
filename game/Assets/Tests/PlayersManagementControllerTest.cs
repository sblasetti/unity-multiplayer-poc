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
        private GameObject fakePlayerPrefab = new GameObject();
        private Mock<IUnityGameObjectProxy> unityGameObjectProxyMock = new Mock<IUnityGameObjectProxy>();
        private Mock<IUnityObjectProxy> unityObjectProxyMock = new Mock<IUnityObjectProxy>();
        private Mock<IUnityDebugProxy> unityDebugProxyMock = new Mock<IUnityDebugProxy>();
        private Mock<IGameState> gameStateMock = new Mock<IGameState>();

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
            gameStateMock.Reset();

            controller = new PlayersManagementController(
                unityGameObjectProxyMock.Object, unityObjectProxyMock.Object, unityDebugProxyMock.Object);
            controller.SetPlayerPrefab(fakePlayerPrefab);
            controller.SetState(gameStateMock.Object);

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
            var socketEvent = BuildSocketIOEventWithPlayerId("test", "TEST_ID");

            // When
            controller.OnPlayerAdded(socketEvent);

            AssertARemotePlayerIsCreated("TEST_ID");
            unityDebugProxyMock.Verify(x => x.Log(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void OnPlayerGone_RemoveRemotePlayerFromLocalGame()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayerId("test", "TEST_ID");
            var fakeGameObject = ObjectMother.BuildGenericGameObject();
            unityGameObjectProxyMock.Setup(x => x.Find(It.IsAny<string>())).Returns(fakeGameObject);

            // When
            controller.OnPlayerGone(socketEvent);

            // Then: remote player object removed from game
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Once);
            unityObjectProxyMock.Verify(x => x.Destroy(fakeGameObject), Times.Once);
            // Then: player removed from the state
            gameStateMock.Verify(x => x.RemoveRemotePlayer("TEST_ID"), Times.Once);
        }

        [Test]
        public void OnOtherPlayersReceived_ShouldAddRemotePlayers()
        {
            // Given
            var players = new List<JSONObject> {
                BuildJSONObjectWithPlayerId("1"),
                BuildJSONObjectWithPlayerId("2"),
                BuildJSONObjectWithPlayerId("3"),
            };
            var listObj = new JSONObject(JSONObject.Type.ARRAY);
            listObj.list = players;
            var jobj = new JSONObject(new Dictionary<string, JSONObject> { { "players", listObj } });
            var socketEvent = ObjectMother.BuildSocketIOEvent("test", jobj);
            var data = socketEvent.data.GetField("players");

            // When
            controller.OnOtherPlayersReceived(socketEvent);

            // Then
            AssertRemotePlayersAreCreated(players.Count);
        }

        private void AssertARemotePlayerIsCreated(string playerId)
        {
            // Then: remote player created as local GameObject using player prefab
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab), Times.Once);
            // Then: player added to the list of remotes
            gameStateMock.Verify(x => x.AddRemotePlayer(playerId), Times.Once);
        }

        private void AssertRemotePlayersAreCreated(int numberOfPlayers)
        {
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab), Times.Exactly(numberOfPlayers));
            gameStateMock.Verify(x => x.AddRemotePlayer(It.IsAny<string>()), Times.Exactly(numberOfPlayers));
        }

        private static SocketIOEvent BuildSocketIOEventWithPlayerId(string eventName, string playerId)
        {
            var jobj = BuildJSONObjectWithPlayerId(playerId);
            var socketEvent = ObjectMother.BuildSocketIOEvent(eventName, jobj);
            return socketEvent;
        }

        private static JSONObject BuildJSONObjectWithPlayerId(string playerId)
        {
            var jobj = ObjectMother.BuildEmptyJSONObject();
            jobj.AddField("id", playerId);
            return jobj;
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
    }

}
