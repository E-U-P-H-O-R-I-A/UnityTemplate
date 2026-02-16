using UnityEngine;

namespace Data.Model
{
    public abstract class IPublicModel : ScriptableObject
    {
        public abstract string Id { get; }
    }
}