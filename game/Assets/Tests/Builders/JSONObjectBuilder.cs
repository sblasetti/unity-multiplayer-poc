using System;
using System.Collections.Generic;
using SocketIO;

namespace Tests
{
    public class JSONObjectBuilder : GenericBuilder<JSONObject>
    {
        public JSONObjectBuilder(Dictionary<string, string> dict)
        {
            this.obj = new JSONObject(dict);
        }

        public static JSONObjectBuilder Dictionary()
        {
            return new JSONObjectBuilder(new Dictionary<string, string>());
        }

        public JSONObjectBuilder WithPlayerId(string value)
        {
            this.obj.AddField(SOCKET_DATA_FIELDS.PlayerId, value);
            return this;
        }

        internal JSONObjectBuilder WithMovementCoordinates(float horizontal, float vertical)
        {
            this.obj.AddField(SOCKET_DATA_FIELDS.HorizontalMovement, horizontal.ToString());
            this.obj.AddField(SOCKET_DATA_FIELDS.VerticalMovement, vertical.ToString());
            return this;
        }

        internal JSONObjectBuilder WithPositionObject(float x, float y)
        {
            var posObj = Dictionary().WithPosition(x, y).Build();
            this.obj.AddField(SOCKET_DATA_FIELDS.Position, posObj);
            return this;
        }

        internal JSONObjectBuilder WithPosition(float x, float y)
        {
            this.obj.AddField(SOCKET_DATA_FIELDS.PositionX, x);
            this.obj.AddField(SOCKET_DATA_FIELDS.PositionY, y);
            return this;
        }
    }
}