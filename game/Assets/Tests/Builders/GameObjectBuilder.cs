using UnityEngine;

namespace Game.Tests.Builders
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
            this.obj.AddComponent<Rigidbody>();
            return this;
        }

        public GameObjectBuilder WithLocalMovement()
        {
            this.obj.AddComponent<LocalMovement>();
            return this;
        }

        public GameObjectBuilder WithIsGround()
        {
            this.obj.AddComponent<IsGround>();
            return this;
        }
    }

}