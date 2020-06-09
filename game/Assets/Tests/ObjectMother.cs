using UnityEngine;
using SocketIO;
using System.Collections.Generic;

namespace Tests
{
    public static class ObjectMother
    {
        public static GameObject BuildGenericGameObject()
        {
            return new GameObject();
        }

        public static JSONObject BuildGenericJSONObject()
        {
            return new JSONObject(new Dictionary<string, string>() { { "id", "value1" } });
        }

    }
}