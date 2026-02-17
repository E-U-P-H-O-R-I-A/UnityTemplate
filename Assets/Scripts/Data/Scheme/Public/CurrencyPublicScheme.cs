using System;
using UnityEngine;

namespace Data.Scheme.Public
{
    public enum CurrencyType
    {
        Soft = 0,
        Hard = 1,
    }
    
    [Serializable]
    public class CurrencyPublicScheme : BasePublicScheme
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private Sprite sprite;
        
        public Sprite Sprite => sprite;
        public override int ID => (int)type;
    }
}