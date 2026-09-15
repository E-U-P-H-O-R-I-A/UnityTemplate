using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility.Factory
{
    public interface IFactory
    {
        UniTask<TObject> CreateFromAssets<TObject>(string key);
        TObject CreateFromPrefab<TObject>(TObject prefab, Transform parent = null)
            where TObject : Component;
    }
}
