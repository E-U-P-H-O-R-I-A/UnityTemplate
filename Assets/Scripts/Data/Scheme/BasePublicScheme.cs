using System;
using UnityEngine;

namespace Data.Scheme
{
    [Serializable]
    public abstract class BasePublicScheme : IScheme
    {
        [SerializeField] private string id;

        public string ID => id;
    }
}