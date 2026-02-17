using System.Collections.Generic;
using Data.Model;
using UnityEngine;

namespace Services.Provider.Public
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private Dictionary<System.Type, BasePublicModel> models = new();

        public void Init()
        {
            var loaded = Resources.LoadAll<BasePublicModel>("Data");

            models = new Dictionary<System.Type, BasePublicModel>(loaded.Length);

            foreach (var model in loaded)
            {
                if (model == null) continue;

                if (models.ContainsKey(model.GetType()))
                {
                    continue;
                }

                models.Add(model.GetType(), model);
            }
        }

        public bool GetModel<TModel>(out TModel model) where TModel : BasePublicModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
            {
                model = typed;
                return true;
            }
            
            model = null;
            return false;
        }
    }
}