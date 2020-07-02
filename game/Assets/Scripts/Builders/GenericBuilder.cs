using System.Xml.Serialization;

namespace Assets.Scripts.Builders
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