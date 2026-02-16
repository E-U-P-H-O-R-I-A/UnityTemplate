using Data.Model;

namespace Services.Provider.Public
{
    public interface IPublicModelProvider : IService
    {
        public void Init();
        public bool GetModel(string id, out IPublicModel model);
    }
}