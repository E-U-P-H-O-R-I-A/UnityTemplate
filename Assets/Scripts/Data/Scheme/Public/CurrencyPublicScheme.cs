using System;
using UnityEngine;

namespace Data
{
    public enum CurrencyType
    {
        Soft = 0,
        Hard = 1,
    }
    
    [Serializable]
    public class CurrencyPublicScheme : PublicScheme
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private Sprite sprite;
        
        public Sprite Sprite => sprite;
        public override string ID => type.ToString();
    }
}