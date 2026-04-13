using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility.Factory
{
    public interface IFactory
    {
        TObject Create<TObject>();
        UniTask<TObject> CreateFromAssets<TObject>(string key);
        TObject CreateFromPrefab<TObject>(TObject prefab, Transform parent = null)
            where TObject : Component;
    }
}