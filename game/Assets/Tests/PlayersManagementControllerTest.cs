using NUnit.Framework;
using Zenject;
using Moq;
using System.Collections.Generic;
using UnityEngine;

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
        public void Init()
        {
            controller = new PlayersManagementController(unityObjectProxyMock.Object, unityDebugProxyMock.Object);

            unityObjectProxyMock.Setup(x => x.Instantiate(It.IsAny<GameObject>())).Returns(ObjectMother.BuildGenericGameObject());
        }

        [Test]
        public void OnPlayerAdded_TBD()
        {
            // Given
            var socketEvent = new SocketIO.SocketIOEvent("test", ObjectMother.BuildGenericJSONObject());

            // When
            controller.OnPlayerAdded(socketEvent);

            // Then
            unityDebugProxyMock.Verify(x => x.Log(It.IsAny<string>()), Times.Once);
            // assert remote player created
            // player added to the list of remotes
        }
    }

}
