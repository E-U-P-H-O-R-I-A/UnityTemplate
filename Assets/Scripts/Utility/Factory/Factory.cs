using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using UnityEngine;
using Zenject;

namespace Utility.Factory
{
    public class Factory : IFactory
    {
        private readonly DiContainer diContainer;
        private readonly IInstantiator instantiator;
        private readonly IAssetsProvider assetsProvider;

        public Factory(DiContainer diContainer, IInstantiator instantiator, IAssetsProvider assetsProvider)
        {
            this.diContainer = diContainer;
            this.instantiator = instantiator;
            this.assetsProvider = assetsProvider;
        }

        public TObject Create<TObject>() => 
            instantiator.Instantiate<TObject>();

        public async UniTask<TObject> CreateFromAssets<TObject>(string key)
        {
            var prefab = await assetsProvider.Load<GameObject>(key);

            return diContainer
                .InstantiatePrefab(prefab)
                .GetComponent<TObject>();
        }
    }
}