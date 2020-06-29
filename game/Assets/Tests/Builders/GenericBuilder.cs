using System.Xml.Serialization;

namespace Game.Tests.Builders
{
    public class GenericBuilder<T>
    {
        protected T obj;

        public T Build()
        {
            return this.obj;
        }
    }
}