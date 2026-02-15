using System.Collections.Generic;
using System.Linq;
using Data.Scheme;
using UnityEngine;

namespace Data.Model
{
    public abstract class BasePublicModel<TScheme> : ScriptableObject where TScheme : BasePublicScheme
    {
        protected abstract List<TScheme> Schemes { get; }
            
        public bool GetScheme(string id, out TScheme scheme)
        {
            scheme = Schemes.FirstOrDefault(scheme => scheme.ID == id);
            return scheme == null;
        }
    }
}