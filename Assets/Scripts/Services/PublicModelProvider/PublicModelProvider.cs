using System.Collections.Generic;
using System.Linq;
using Data.Model;
using UnityEngine;

namespace Services.Provider.Public
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private Dictionary<System.Type, IPublicModel> models = new();

        public void Init()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("Data");

            models = loaded
                .OfType<IPublicModel>()     
                .GroupBy(m => m.GetType())
                .ToDictionary(g => g.Key, g => g.First());
        }

        public TModel GetModel<TModel>() where TModel : IPublicModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
            {
                return typed;
            }
            
            return default;
        }
    }
}