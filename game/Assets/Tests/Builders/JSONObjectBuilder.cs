using System;
using System.Collections.Generic;
using SocketIO;

namespace Game.Tests.Builders
{
    public class JSONObjectBuilder : GenericBuilder<JSONObject>
    {
        public JSONObjectBuilder()
        {
            this.obj = new JSONObject();
        }

        public static JSONObjectBuilder Empty()
        {
            return new JSONObjectBuilder();
        }

        public JSONObjectBuilder WithPayload(JSONObject obj)
        {
            this.obj.AddField(SOCKET_DATA_FIELDS.Payload, obj);
            return this;
        }

        public JSONObject WrapAsPayload()
        {
            JSONObject wrap = new JSONObject();
            wrap.AddField(SOCKET_DATA_FIELDS.Payload, this.obj);
            return wrap;
        }

        public JSONObjectBuilder WithField(string field, List<JSONObject> list)
        {
            this.obj.AddField(field, new JSONObject(list.ToArray()));
            return this;
        }

        public JSONObjectBuilder WithField(string field, JSONObject obj)
        {
            this.obj.AddField(field, obj);
            return this;
        }

        public JSONObjectBuilder WithField(string field, string obj)
        {
            this.obj.AddField(field, obj);
            return this;
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
            var posObj = Empty().WithPosition(x, y).Build();
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