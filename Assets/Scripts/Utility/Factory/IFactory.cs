using Cysharp.Threading.Tasks;

namespace Utility.Factory
{
    public interface IFactory
    {
        TObject Create<TObject>();
        UniTask<TObject> CreateFromAssets<TObject>(string key);
    }
}