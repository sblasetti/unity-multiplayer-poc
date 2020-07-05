using Assets.Scripts.Builders;
using SocketIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public interface INetworkController
    {
        void SendLocalPosition(GameObject player);
        void SetSocket(ISocketIOComponent socketIOComponent);
    }

    public class NetworkController : INetworkController
    {
        private ISocketIOComponent socket;
        private IGameEventBuilder eventPayloadBuilder;

        public NetworkController(IGameEventBuilder eventPayloadBuilder)
        {
            this.eventPayloadBuilder = eventPayloadBuilder;
        }

        public void SendLocalPosition(GameObject player)
        {
            if (socket != null)
            {
                var gameEvent = eventPayloadBuilder.BuildPlayerLocalMove(player);
                Send(gameEvent);
            }
        }

        public void SetSocket(ISocketIOComponent socketIOComponent)
        {
            this.socket = socketIOComponent;
        }

        private void Send(GameEvent e)
        {
            socket.EmitIfConnected(e.Name, e.Payload);
        }
    }
}
