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
        void SendLocalPositionChange(float distanceChange, float directionChange);
        void SetSocket(ISocketIOComponent socketIOComponent);
    }

    public class NetworkController : INetworkController
    {
        private ISocketIOComponent socket;

        public void SendLocalPositionChange(float distanceChange, float directionChange)
        {
            if (socket != null)
            {
                var data = BuildPositionData(directionChange, distanceChange);
                socket.EmitIfConnected(SOCKET_EVENTS.PlayerLocalMove, data);
            }
        }

        private static JSONObject BuildPositionData(float direction, float distance)
        {
            // TODO: improve
            var payload = new JSONObject();
            payload.AddField(SOCKET_DATA_FIELDS.DirectionChange, direction);
            payload.AddField(SOCKET_DATA_FIELDS.DistanceChange, distance);
            var jobj = new JSONObject();
            jobj.AddField(SOCKET_DATA_FIELDS.Payload, payload);
            return jobj;
        }

        public void SetSocket(ISocketIOComponent socketIOComponent)
        {
            this.socket = socketIOComponent;
        }
    }
}
