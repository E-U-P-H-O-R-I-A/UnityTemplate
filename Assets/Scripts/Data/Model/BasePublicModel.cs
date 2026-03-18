using System.Collections.Generic;
using System.Linq;
using Data.Scheme;
using UnityEngine;

namespace Data.Model
{
    public abstract class BasePublicModel<TScheme> : ScriptableObject, IPublicModel where TScheme : BasePublicScheme
    {
        [SerializeField] private List<TScheme> schemes;
        
        public IReadOnlyList<TScheme> Schemes => schemes;
            
        public TScheme GetScheme(int id)
        {
            return schemes.FirstOrDefault(scheme => scheme.ID == id);;
        }
    }
}