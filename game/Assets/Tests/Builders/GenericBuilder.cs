using System.Xml.Serialization;

namespace Tests
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