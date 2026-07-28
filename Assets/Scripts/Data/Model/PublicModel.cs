using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    public abstract class PublicModel<TScheme> : ScriptableObject, IPublicModel where TScheme : PublicScheme
    {
        [Searchable, ListDrawerSettings(Expanded = true, ListElementLabelName = "ID")] 
        [SerializeField] protected List<TScheme> schemes;

        public IReadOnlyList<TScheme> Schemes => schemes;

        public virtual TScheme GetScheme(string id)
        {
            return schemes.FirstOrDefault(scheme => scheme != null && scheme.ID == id);
        }
    }
}
