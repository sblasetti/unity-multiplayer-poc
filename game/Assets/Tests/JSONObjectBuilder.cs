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
    }
}