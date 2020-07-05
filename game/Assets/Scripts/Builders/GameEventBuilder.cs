using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Builders
{
    public class GameEvent {
        public string Name { get; set; }
        public JSONObject Payload { get; set; }
    }

    public interface IGameEventBuilder
    {
        GameEvent BuildPlayerLocalMove(GameObject player);
    }

    public class GameEventBuilder : IGameEventBuilder
    {
        public GameEvent BuildPlayerLocalMove(GameObject player)
        {
            var position = player.transform.position;
            var rotation = player.transform.rotation;
            var payload = JSONObjectBuilder.Empty()
                .WithPositionObject(position.x, position.y, position.z)
                .WithRotationObject(rotation.x, rotation.y, rotation.z, rotation.w)
                .WrapAsPayload();

            return new GameEvent { 
                Name = SOCKET_EVENTS.PlayerLocalMove,
                Payload = payload
            };

        }
    }
}
