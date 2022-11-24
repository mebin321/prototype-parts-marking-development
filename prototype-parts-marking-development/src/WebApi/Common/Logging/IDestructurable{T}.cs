namespace WebApi.Common.Logging
{
    public interface IDestructurable<out T>
    {
        T Destructure();
    }
}
