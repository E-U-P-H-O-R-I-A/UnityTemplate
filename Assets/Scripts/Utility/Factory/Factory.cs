using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Utility.Factory
{
    public class Factory : IFactory
    {
        private readonly IObjectResolver resolver;
        private readonly IAssetsProvider assetsProvider;

        public Factory(IObjectResolver resolver, IAssetsProvider assetsProvider)
        {
            this.resolver = resolver;
            this.assetsProvider = assetsProvider;
        }

        public TObject Create<TObject>() => 
            resolver.Resolve<TObject>();

        public async UniTask<TObject> CreateFromAssets<TObject>(string key)
        {
            var prefab = await assetsProvider.Load<GameObject>(key);

            return resolver
                .Instantiate(prefab)
                .GetComponent<TObject>();
        }
    }
}