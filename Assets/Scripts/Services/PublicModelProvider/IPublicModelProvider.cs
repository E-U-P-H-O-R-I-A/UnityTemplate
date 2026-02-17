using Data.Model;

namespace Services.Provider.Public
{
    public interface IPublicModelProvider : IService
    {
        public void Init();
        public TModel GetModel<TModel>() where TModel : IPublicModel;
    }
}