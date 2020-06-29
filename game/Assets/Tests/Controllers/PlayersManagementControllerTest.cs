using NUnit.Framework;
using Zenject;
using Moq;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Linq;
using Game.Tests.Builders;

namespace Game.Tests.Controllers
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
        private Mock<IInstantiator> instantiatorMock = new Mock<IInstantiator>();
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
            instantiatorMock.Reset();

            fakePlayerPrefab = GameObjectBuilder.New().Build();

            controller = new PlayersManagementController(
                unityGameObjectProxyMock.Object, unityObjectProxyMock.Object, unityDebugProxyMock.Object, instantiatorMock.Object);
            controller.SetPlayerPrefab(fakePlayerPrefab);
            controller.SetState(gameStateMock.Object);

            socketMock = new Mock<ISocketIOComponent>();
            controller.SetSocket(socketMock.Object);

            socketMock.Setup(x => x.IsConnected).Returns(true);
            instantiatorMock.Setup(x => x.InstantiatePrefab(It.IsAny<GameObject>(), It.IsAny<Vector3>(), It.IsAny<Quaternion>(), null)).Returns(fakeLocalPlayer);
            
        }

        [Test]
        public void OnPlayerWelcome_SpawnLocalPlayerAtPositionAndInformTheServer()
        {
            // Given
            float posX = 12.2F, posY = 4.1F;
            var socketEvent = BuildPlayerInitialPositionSocketEvent(posX, posY);

            // When
            controller.OnPlayerWelcome(socketEvent);

            // Then
            var expectedPos = new Vector3(posX, 0, posY);
            AssertLocalPlayerIsCreated(expectedPos);
            socketMock.Verify(x => x.Emit(SOCKET_EVENTS.PlayerJoin, It.IsAny<JSONObject>()), Times.Once);
        }

        [Test]
        public void OnPlayerWelcomeAndAlreadySpawned_DoNothing()
        {
            // Given
            float posX = 12.2F, posY = 4.1F;
            var socketEvent = BuildPlayerInitialPositionSocketEvent(posX, posY);
            controller.OnPlayerWelcome(socketEvent);
            Assert.AreEqual(controller.GetLocalPlayer(), fakeLocalPlayer);
            unityObjectProxyMock.Invocations.Clear();
            socketMock.Invocations.Clear();

            // When
            controller.OnPlayerWelcome(socketEvent);

            // Then
            unityObjectProxyMock.Verify(x => x.Instantiate(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity), Times.Never);
            socketMock.Verify(x => x.Emit(It.IsAny<string>(), It.IsAny<JSONObject>()), Times.Never);
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
            AssertRemotePlayerIsCreatedLocally("TEST_ID", 2.2F, 3.3F);
        }

        [Test]
        public void OnPlayerAddedAndAlreadyExists_PlayerIsNotCreated()
        {
            // Given
            var socketEvent = BuildSocketIOEventWithPlayer(SOCKET_EVENTS.PlayerNew, "TEST_ID", 2.2F, 3.3F);
            controller.OnPlayerAdded(socketEvent);
            instantiatorMock.Invocations.Clear();
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

        private static SocketIOEvent BuildPlayerInitialPositionSocketEvent(float posX, float posY)
        {
            var data = JSONObjectBuilder.Empty()
                .WithPosition(posX, posY)
                .WrapAsPayload();

            var socketEvent = SocketIOEventBuilder.Empty(SOCKET_EVENTS.PlayerWelcome)
                .WithData(data)
                .Build();

            return socketEvent;
        }

        private void AssertLocalPlayerIsCreated(Vector3 expectedPos)
        {
            instantiatorMock.Verify(x => x.InstantiatePrefab(fakePlayerPrefab, expectedPos, Quaternion.identity, null), Times.Once);
            instantiatorMock.Verify(x => x.InstantiateComponent<LocalMovement>(fakeLocalPlayer), Times.Once);
            Assert.AreEqual(controller.GetLocalPlayer(), fakeLocalPlayer);
        }

        private void AssertRemotePlayerIsCreatedLocally(string playerId, float posX, float posY)
        {
            // Then: remote player created as local GameObject using player prefab
            instantiatorMock.Verify(x => x.InstantiatePrefab(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity, null), Times.Once);
            instantiatorMock.Verify(x => x.InstantiateComponent<LocalMovement>(fakeLocalPlayer), Times.Never);
            // Then: player added to the list of remotes
            gameStateMock.Verify(x => x.AddRemotePlayer(playerId,  posX, posY), Times.Once);
        }

        private void AssertRemotePlayersAreCreatedLocally(int numberOfPlayers)
        {
            instantiatorMock.Verify(x => x.InstantiatePrefab(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity, null), Times.Exactly(numberOfPlayers));
            instantiatorMock.Verify(x => x.InstantiateComponent<LocalMovement>(fakeLocalPlayer), Times.Never);
            gameStateMock.Verify(x => x.AddRemotePlayer(It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()), Times.Exactly(numberOfPlayers));
        }

        private void AssertRemotePlayerIsNotCreatedLocally()
        {
            instantiatorMock.Verify(x => x.InstantiatePrefab(fakePlayerPrefab, It.IsAny<Vector3>(), Quaternion.identity, null), Times.Never);
            gameStateMock.Verify(x => x.AddRemotePlayer(It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()), Times.Never);
        }

        private static SocketIOEvent BuildOnPlayerReceivedEvent(List<JSONObject> players)
        {
            var jobj = JSONObjectBuilder.Empty()
                .WithField(SOCKET_DATA_FIELDS.Payload, players)
                .Build();
            var socketEvent = SocketIOEventBuilder.New("test").WithData(jobj).Build();
            var data = socketEvent.data.GetField("players");
            return socketEvent;
        }

        private static SocketIOEvent BuildSocketIOEventWithPlayer(string eventName, string playerId, float posX = 0F, float posY = 0F)
        {
            var jobj = JSONObjectBuilder.Empty()
                .WithPayload(BuildJSONObjectWithPlayerId(playerId, posX, posY))
                .Build();
            var socketEvent = SocketIOEventBuilder.New(eventName).WithData(jobj).Build();
            return socketEvent;
        }

        private static JSONObject BuildJSONObjectWithPlayerId(string playerId, float posX = 0F, float posY = 0F)
        {
            var jobj = JSONObjectBuilder.Empty()
                .WithPlayerId(playerId)
                .WithField(SOCKET_DATA_FIELDS.PlayerId, playerId)
                .WithPositionObject(posX, posY)
                .Build();

            return jobj;
        }
    }

}
