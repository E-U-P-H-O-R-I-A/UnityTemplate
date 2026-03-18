using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Data.Model;
using Services.AssetProvider;

namespace Services.PublicModelProvider
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private readonly IAssetsProvider assetsProvider;
        
        private Dictionary<System.Type, IPublicModel> models = new();

        public PublicModelProvider(IAssetsProvider assetsProvider)
        {
            this.assetsProvider = assetsProvider;
        }
        
        public async UniTask Init(CancellationToken ct = default)
        {
            var keys = await assetsProvider.GetAssetsListByLabel<IPublicModel>(AssetsLabels.DATA);
            ct.ThrowIfCancellationRequested();
            var loaded = await assetsProvider.LoadAll<IPublicModel>(keys);
            ct.ThrowIfCancellationRequested();
            
            models = loaded
                .Where(m => m != null)
                .GroupBy(m => m.GetType())
                .ToDictionary(g => g.Key, g => g.First());
        }

        public TModel GetModel<TModel>() where TModel : IPublicModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
            {
                return typed;
            }
            
            return default;
        }
    }
}