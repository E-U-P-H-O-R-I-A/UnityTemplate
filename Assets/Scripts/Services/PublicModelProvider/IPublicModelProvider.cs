using System.Threading;
using Cysharp.Threading.Tasks;
using Data.Model;

namespace Services.PublicModelProvider
{
    public interface IPublicModelProvider : IService
    {
        public UniTask Initialize(CancellationToken ct = default);
        public TModel GetModel<TModel>() where TModel : IPublicModel;
    }
}