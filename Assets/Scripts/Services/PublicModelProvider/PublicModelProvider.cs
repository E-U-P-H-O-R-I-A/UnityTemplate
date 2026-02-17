using System.Collections.Generic;
using Data.Model;
using UnityEngine;

namespace Services.Provider.Public
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private Dictionary<System.Type, IPublicModel> models = new();

        public void Init()
        {
            var loaded = Resources.LoadAll<IPublicModel>("Data");

            models = new Dictionary<System.Type, IPublicModel>(loaded.Length);

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

        public bool GetModel<TModel>(out TModel model) where TModel : IPublicModel
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