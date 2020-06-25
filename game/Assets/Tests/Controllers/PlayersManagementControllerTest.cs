using NUnit.Framework;
using Zenject;
using Moq;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class PlayersManagementControllerTest : ZenjectUnitTestFixture
    {
        private IPlayersManagementController controller;
        private GameObject fakePlayerPrefab = null;
        private GameObject fakeLocalPlayer = null;
        private GameObject fakeRemotePlayer = null;
        private Mock<IUnityGameObjectProxy> unityGameObjectProxyMock = new Mock<IUnityGameObjectProxy>();
        private Mock<IUnityObjectProxy> unityObjectProxyMock = new Mock<IUnityObjectProxy>();
        private Mock<IUnityDebugProxy> unityDebugProxyMock = new Mock<IUnityDebugProxy>();
        private Mock<IGameState> gameStateMock = new Mock<IGameState>();
        private Mock<ISocketIOComponent> socketMock = null;

        /* TODO: investigate how to initialize the DI container that the test has as property

            Assert.IsNotNull(Container);
            Container.Bind<IUnityObjectProxy>().FromInstance(unityObjectProxyMock.Object);
            Container.Bind<IUnityDebugProxy>().FromInstance(unityDebugProxyMock.Object);
            Container.Bind<IPlayersManagementController>().AsSingle();

            Container.Inject(controller);

        */

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            fakeLocalPlayer = GameObjectBuilder.New().Build();
            fakeRemotePlayer = GameObjectBuilder.New().Build();
        }

        [SetUp]
        public void SetUp()
        {
            unityDebugProxyMock.Reset();
            unityObjectProxyMock.Reset();
            unityGameObjectProxyMock.Reset();
            gameStateMock.Reset();

            fakePlayerPrefab = GameObjectBuilder.New().Build();

            controller = new PlayersManagementController(
                unityGameObjectProxyMock.Object, unityObjectProxyMock.Object, unityDebugProxyMock.Object);
            controller.SetPlayerPrefab(fakePlayerPrefab);
            controller.SetState(gameStateMock.Object);

            socketMock = new Mock<ISocketIOComponent>();
            controller.SetSocket(socketMock.Object);

            unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>(), It.IsAny<Vector3>(), It.IsAny<Quaternion>())).Returns(fakeLocalPlayer);
        }

        [Test]
        public void OnPlayerInitialPosition_SpawnLocalPlayerAtPosition()
        {
            // Given
            float posX = 12.2F, posY = 4.1F;
            var socketEvent = BuildPlayerInitialPositionSocketEvent(posX, posY);

            // When
            controller.OnPlayerInitialPosition(socketEvent);

            // Then
            var expectedPos = new Vector3(posX, 0, posY);
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, expectedPos, Quaternion.identity), Times.Once);
            Assert.AreEqual(controller.GetLocalPlayer(), fakeLocalPlayer);
            socketMock.Verify(x => x.Emit(It.IsAny<string>(), It.IsAny<JSONObject>()), Times.Never);
        }

        [Test]
        public void OnPlayerInitialPositionAndAlreadySpawned_DoNothing()
        {
            // Given
            float posX = 12.2F, posY = 4.1F;
            var socketEvent = BuildPlayerInitialPositionSocketEvent(posX, posY);
            controller.OnPlayerInitialPosition(socketEvent);
            Assert.AreEqual(controller.GetLocalPlayer(), fakeLocalPlayer);
            unityObjectProxyMock.Reset();

            // When
            controller.OnPlayerInitialPosition(socketEvent);

            // Then
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity), Times.Never);
            socketMock.Verify(x => x.Emit(It.IsAny<string>(), It.IsAny<JSONObject>()), Times.Never);
        }

        private static SocketIOEvent BuildPlayerInitialPositionSocketEvent(float posX, float posY)
        {
            var data = JSONObjectBuilder.Dictionary()
                .WithPosition(posX, posY)
                .Build();

            var socketEvent = SocketIOEventBuilder.Empty(SOCKET_EVENTS.PlayerInitialPosition)
                .WithData(data)
                .Build();

            return socketEvent;
        }

        [Test]
        public void OnPlayerAdded_PlayerIsCreatedLocallyAndAddedToTheListOfRemotePlayers()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayer(SOCKET_EVENTS.PlayerNew, "TEST_ID", 2.2F, 3.3F);

            // When
            controller.OnPlayerAdded(socketEvent);

            // Then
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Once);
            AssertARemotePlayerIsCreatedLocally("TEST_ID", 2.2F, 3.3F);
        }

        [Test]
        public void OnPlayerAddedAndAlreadyExists_PlayerIsNotCreated()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayer(SOCKET_EVENTS.PlayerNew, "TEST_ID", 2.2F, 3.3F);
            controller.OnPlayerAdded(socketEvent);
            unityObjectProxyMock.Invocations.Clear();
            gameStateMock.Invocations.Clear();
            unityGameObjectProxyMock.Setup(x => x.Find("Player:TEST_ID")).Returns(fakeRemotePlayer);

            // When
            controller.OnPlayerAdded(socketEvent);

            // Then
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Exactly(2));
            AssertRemotePlayerIsNotCreatedLocally();
        }

        [Test]
        public void OnPlayerGone_RemoveRemotePlayerFromLocalGame()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayer("test", "TEST_ID", 4.4F, 5.5F);
            var fakeGameObject = GameObjectBuilder.New().Build();
            unityGameObjectProxyMock.Setup(x => x.Find(It.IsAny<string>())).Returns(fakeGameObject);

            // When
            controller.OnPlayerGone(socketEvent);

            // Then: remote player object removed from game
            unityGameObjectProxyMock.Verify(x => x.Find("Player:TEST_ID"), Times.Once);
            unityObjectProxyMock.Verify(x => x.DestroyImmediate(fakeGameObject), Times.Once);
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
            SocketIOEvent socketEvent = BuildOnPlayerReceivedEvent(players);

            // When
            controller.OnOtherPlayersReceived(socketEvent);

            // Then
            AssertRemotePlayersAreCreatedLocally(players.Count);
        }

        [Test]
        public void OnOtherPlayersReceived_ShouldNotAddRemotePlayerIfAlreadyExists()
        {
            // Given
            var players = new List<JSONObject> {
                BuildJSONObjectWithPlayerId("1"),
                BuildJSONObjectWithPlayerId("2"),
                BuildJSONObjectWithPlayerId("3"),
            };
            SocketIOEvent socketEvent = BuildOnPlayerReceivedEvent(players);
            unityGameObjectProxyMock.Setup(x => x.Find("Player:2")).Returns(fakeRemotePlayer);
            gameStateMock.Setup(x => x.RemotePlayerExists("2")).Returns(true);

            // When
            controller.OnOtherPlayersReceived(socketEvent);

            // Then
            AssertRemotePlayersAreCreatedLocally(2);
            gameStateMock.Verify(x => x.AddRemotePlayer("2", It.IsAny<float>(), It.IsAny<float>()), Times.Never);
        }

        private void AssertARemotePlayerIsCreatedLocally(string playerId, float posX, float posY)
        {
            // Then: remote player created as local GameObject using player prefab
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, new Vector3(posX, 0, posY), Quaternion.identity), Times.Once);
            // Then: player added to the list of remotes
            gameStateMock.Verify(x => x.AddRemotePlayer(playerId,  posX, posY), Times.Once);
        }

        private void AssertRemotePlayersAreCreatedLocally(int numberOfPlayers)
        {
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity), Times.Exactly(numberOfPlayers));
            gameStateMock.Verify(x => x.AddRemotePlayer(It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()), Times.Exactly(numberOfPlayers));
        }

        private void AssertRemotePlayerIsNotCreatedLocally()
        {
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity), Times.Never);
            gameStateMock.Verify(x => x.AddRemotePlayer(It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()), Times.Never);
        }

        private static SocketIOEvent BuildOnPlayerReceivedEvent(List<JSONObject> players)
        {
            var listObj = new JSONObject(JSONObject.Type.ARRAY);
            listObj.list = players;
            var jobj = new JSONObject(new Dictionary<string, JSONObject> { { "players", listObj } });
            var socketEvent = SocketIOEventBuilder.New("test").WithData(jobj).Build();
            var data = socketEvent.data.GetField("players");
            return socketEvent;
        }

        private static SocketIOEvent BuildSocketIOEventWithPlayer(string eventName, string playerId, float posX = 0F, float posY = 0F)
        {
            var jobj = BuildJSONObjectWithPlayerId(playerId, posX, posY);
            var socketEvent = SocketIOEventBuilder.New(eventName).WithData(jobj).Build();
            return socketEvent;
        }

        private static JSONObject BuildJSONObjectWithPlayerId(string playerId, float posX = 0F, float posY = 0F)
        {
            var jobj = JSONObjectBuilder.Dictionary()
                .WithPlayerId(playerId)
                .WithPositionObject(posX, posY)
                .Build();
            jobj.AddField(SOCKET_DATA_FIELDS.PlayerId, playerId);
            return jobj;
        }
    }

}
