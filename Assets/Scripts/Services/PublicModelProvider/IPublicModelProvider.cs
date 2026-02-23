using Cysharp.Threading.Tasks;
using Data.Model;

namespace Services.PublicModelProvider
{
    public interface IPublicModelProvider : IService
    {
        public UniTask Init();
        public TModel GetModel<TModel>() where TModel : IPublicModel;
    }
}