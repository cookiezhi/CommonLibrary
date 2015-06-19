namespace Stone.Framework.Common.Collection
{
    public interface IKeyedObject<T>
    {
        T Key
        {
            get;
        }
    }
}