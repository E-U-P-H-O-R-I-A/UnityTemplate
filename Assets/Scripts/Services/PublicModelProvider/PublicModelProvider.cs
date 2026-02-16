using System.Collections.Generic;
using Data.Model;
using UnityEngine;

namespace Services.Provider.Public
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private Dictionary<string, IPublicModel> models = new();

        public void Init()
        {
            var loaded = Resources.LoadAll<IPublicModel>("Data");

            models = new Dictionary<string, IPublicModel>(loaded.Length);

            foreach (var model in loaded)
            {
                if (model == null) continue;

                if (models.ContainsKey(model.Id))
                {
                    continue;
                }

                models.Add(model.Id, model);
            }
        }
        
        public bool GetModel(string id, out IPublicModel model)
        {
            return models.TryGetValue(id, out model);
        }
    }
}