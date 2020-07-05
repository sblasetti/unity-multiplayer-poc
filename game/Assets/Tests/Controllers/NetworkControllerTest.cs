using Assets.Scripts.Builders;
using Assets.Scripts.Controllers;
using Moq;
using NUnit.Framework;
using SocketIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Tests.Controllers
{
    [TestFixture]
    public class NetworkControllerTest
    {
        private INetworkController controller;

        private GameEvent fakeEvent;

        private Mock<ISocketIOComponent> socketMock = new Mock<ISocketIOComponent>();
        private Mock<IGameEventBuilder> gameEventBuilderMock = new Mock<IGameEventBuilder>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BuildFakeEvent();
            SetupSocketMock();
            SetupGameEventBuilderMock();
        }

        private void SetupGameEventBuilderMock()
        {
            gameEventBuilderMock.Setup(x => x.BuildPlayerLocalMove(It.IsAny<GameObject>())).Returns(fakeEvent);
        }

        private void SetupSocketMock()
        {
            socketMock.SetupGet(x => x.IsConnected).Returns(true);
        }

        private void BuildFakeEvent()
        {
            fakeEvent = new GameEvent
            {
                Name = "TEST",
                Payload = JSONObjectBuilder.Empty()
                    .WithField("a", 1.23F)
                    .WrapAsPayload()
            };
        }

        [SetUp]
        public void SetUp()
        {
            ClearMocksInvocations();
            InstantiateController();
        }

        private void InstantiateController()
        {
            controller = new NetworkController(gameEventBuilderMock.Object);
        }

        private void ClearMocksInvocations()
        {
            socketMock.Invocations.Clear();
            gameEventBuilderMock.Invocations.Clear();
        }

        [Theory]
        public void SendLocalPositionChange_ShouldSendEvent(bool hasSocketInstance)
        {
            // Given
            var fakePlayer = new GameObject();
            var fakeDistanceChange = 0.41F;
            var fakeDirectionChange = -0.22F;
            if (hasSocketInstance) controller.SetSocket(socketMock.Object);

            // When
            controller.SendLocalPosition(fakePlayer);

            // Then
            var times = hasSocketInstance ? Times.Once() : Times.Never();
            gameEventBuilderMock.Verify(x => x.BuildPlayerLocalMove(fakePlayer), times);
            socketMock.Verify(x => x.Emit(fakeEvent.Name, fakeEvent.Payload), times);
        }
    }
}
