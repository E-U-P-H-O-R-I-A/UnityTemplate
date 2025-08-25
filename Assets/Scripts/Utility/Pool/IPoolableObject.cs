namespace Utility.Pool
{
    public interface IPoolableObject
    {
        void OnPop();
        void OnPush();
    }
}