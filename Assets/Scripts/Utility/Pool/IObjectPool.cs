using Cysharp.Threading.Tasks;

namespace Utility.Pool
{
    public interface IObjectPool<T> where T : IPoolableObject
    {
        UniTask<T> Pop(string path);
        void Push(T item);
    }
}