using UnityEngine;

namespace Tests
{

    public class GameObjectBuilder : GenericBuilder<GameObject>
    {
        public GameObjectBuilder()
        {
            this.obj = new GameObject();
        }

        public static GameObjectBuilder New()
        {
            return new GameObjectBuilder();
        }
        public GameObjectBuilder WithPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            this.obj.transform.position = position;
            this.obj.transform.rotation = rotation;
            return this;
        }

        public GameObjectBuilder WithRigidbody()
        {
            var rb = this.obj.AddComponent<Rigidbody>();
            return this;
        }
    }

}