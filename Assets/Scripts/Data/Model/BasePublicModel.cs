using System.Collections.Generic;
using System.Linq;
using Data.Scheme;
using UnityEngine;

namespace Data.Model
{
    public abstract class BasePublicModel<TScheme> : IPublicModel where TScheme : BasePublicScheme
    {
        [SerializeField] private List<TScheme> schemes;
        
        public List<string> GetAllId()
        {
            return schemes.Select(scheme => scheme.ID).ToList();
        }
            
        public bool GetScheme(string id, out TScheme scheme)
        {
            scheme = schemes.FirstOrDefault(scheme => scheme.ID == id);
            return scheme != null;
        }
    }
}