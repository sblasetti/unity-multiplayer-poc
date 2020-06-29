using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;

namespace Game.Tests.Builders
{
    public class SocketIOEventBuilder : GenericBuilder<SocketIOEvent>
    {
        public static SocketIOEventBuilder New(string eventName)
        {
            return new SocketIOEventBuilder(eventName);
        }

        public static SocketIOEventBuilder Empty(string eventName)
        {
            return new SocketIOEventBuilder(eventName).WithEmptyData();
        }

        private SocketIOEventBuilder(string name)
        {
            this.obj = new SocketIOEvent(name);
        }

        public SocketIOEventBuilder WithEmptyData()
        {
            this.obj.data = new JSONObject(new Dictionary<string, string>());
            return this;
        }

        public SocketIOEventBuilder WithData(JSONObject data)
        {
            this.obj.data = data;
            return this;
        }
    }
}