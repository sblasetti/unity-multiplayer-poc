using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Builders
{
    public class GameEvent {
        public string Name { get; set; }
        public JSONObject Payload { get; set; }
    }

    public interface IGameEventBuilder
    {
        GameEvent BuildPlayerLocalMove(float distanceChange, float directionChange);
    }

    public class GameEventBuilder : IGameEventBuilder
    {
        public GameEvent BuildPlayerLocalMove(float distanceChange, float directionChange)
        {
            var payload = JSONObjectBuilder.Empty()
                .WithField(SOCKET_DATA_FIELDS.DirectionChange, directionChange)
                .WithField(SOCKET_DATA_FIELDS.DistanceChange, distanceChange)
                .WrapAsPayload();

            return new GameEvent { 
                Name = SOCKET_EVENTS.PlayerLocalMove,
                Payload = payload
            };

        }
    }
}
