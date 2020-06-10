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
            controller = new PlayersManagementController(
                unityObjectProxyMock.Object, unityDebugProxyMock.Object);

            unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>())).Returns(ObjectMother.BuildGenericGameObject());
        }

        [Test]
        public void OnConnectionOpen_ShouldSendPlayerDataToTheServer()
        {
            // Given
            var socketMock = new Mock<ISocketIOComponent>();
            controller.SetSocket(socketMock.Object);

            // When
            var socketEvent = ObjectMother.BuildSocketIOEvent("test", ObjectMother.BuildEmptyJSONObject());
            controller.OnConnectionOpen(socketEvent);

            // Then
            socketMock.Verify(x => x.Emit(SocketEvents.PlayerSendData), Times.Once);
        }

        [Test]
        public void OnPlayerAdded_PlayerIsCreatedLocallyAndAddedToTheListOfRemotePlayers()
        {
            // Given
            Assert.AreEqual(controller.PlayersCount, 0);
            var jobj = ObjectMother.BuildEmptyJSONObject();
            jobj.AddField("id", "TEST_ID");
            var socketEvent = ObjectMother.BuildSocketIOEvent("test", jobj);

            // When
            controller.OnPlayerAdded(socketEvent);

            // Then: assert remote player created as local GameObject
            unityObjectProxyMock.Verify(x => x.Instantiate(It.IsAny<GameObject>()), Times.Once);
            // Then: player added to the list of remotes
            Assert.AreEqual(controller.PlayersCount, 1);
            var player = controller.GetRemotePlayer("TEST_ID");
            Assert.IsNotNull(player);
            unityDebugProxyMock.Verify(x => x.Log(It.IsAny<string>()), Times.Once);
        }
    }

}
