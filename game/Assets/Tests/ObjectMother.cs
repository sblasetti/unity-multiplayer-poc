using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;

namespace Tests
{
    public static class ObjectMother
    {
        public static GameObject BuildGenericGameObject()
        {
            return new GameObject();
        }

        public static JSONObject BuildEmptyJSONObject()
        {
            return new JSONObject(new Dictionary<string, string>());
        }

        internal static SocketIOEvent BuildSocketIOEvent(string name, JSONObject jobj)
        {
            return new SocketIO.SocketIOEvent(name, jobj);
        }
    }
}