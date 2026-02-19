using Data.Model;

namespace Services.Provider.Public
{
    public interface IPrivateModelProvider : IService
    {
        public void Init();
        public void SaveAll();
        public void SaveModel<TModel>() where TModel : IPrivateModel;
        public TModel GetModel<TModel>() where TModel : IPrivateModel;
    }
}
