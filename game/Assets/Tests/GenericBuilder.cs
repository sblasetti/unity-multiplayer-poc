public class GenericBuilder<T> where T : new()
{
    protected T obj;

    public GenericBuilder()
    {
        obj = new T();
    }

    public T Build()
    {
        return this.obj;
    }
}