using Data.Model;

namespace Services.Provider.Public
{
    public interface IPublicModelProvider : IService
    {
        public void Init();
        public bool GetModel<TModel>(out TModel model) where TModel : BasePublicModel;
    }
}