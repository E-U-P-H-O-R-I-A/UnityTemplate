using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Services.PrivateModelProvider
{
    public interface IPrivateModelProvider : IService
    {
        public UniTask Initizele(CancellationToken ct = default);
        public UniTask SaveAll(CancellationToken ct = default);
        public UniTask SaveModel<TModel>(CancellationToken ct = default) where TModel : IPrivateModel;
        public TModel GetModel<TModel>() where TModel : IPrivateModel;
    }
}
