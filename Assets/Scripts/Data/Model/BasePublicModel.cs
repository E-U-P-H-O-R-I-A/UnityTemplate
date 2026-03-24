using System.Collections.Generic;
using System.Linq;
using Data.Scheme;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.Model
{
    public abstract class BasePublicModel<TScheme> : ScriptableObject, IPublicModel where TScheme : BasePublicScheme
    {
        [Searchable, ListDrawerSettings(Expanded = true, ListElementLabelName = "StringID")]
        [SerializeField] private List<TScheme> schemes;
        
        public IReadOnlyList<TScheme> Schemes => schemes;
            
        public TScheme GetScheme(int id)
        {
            return schemes.FirstOrDefault(scheme => scheme.ID == id);;
        }
    }
}