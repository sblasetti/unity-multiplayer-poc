using System.Collections.Generic;
using SocketIO;

namespace Tests
{
    public class JSONObjectBuilder
    {
        private JSONObject jobj;
        public JSONObjectBuilder()
        {
            this.jobj = new JSONObject();
        }

        public JSONObjectBuilder(Dictionary<string, string> dict)
        {
            this.jobj = new JSONObject(dict);
        }

        public static JSONObjectBuilder Empty()
        {
            return new JSONObjectBuilder();
        }

        public static JSONObjectBuilder Dictionary()
        {
            return new JSONObjectBuilder(new Dictionary<string, string>());
        }

        public JSONObject Build()
        {
            return this.jobj;
        }
    }
}