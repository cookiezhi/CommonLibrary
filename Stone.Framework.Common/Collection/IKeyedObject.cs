namespace Stone.Framework.Common.Collection
{
    public interface IKeyedObject<out T>
    {
        T Key
        {
            get;
        }
    }
}