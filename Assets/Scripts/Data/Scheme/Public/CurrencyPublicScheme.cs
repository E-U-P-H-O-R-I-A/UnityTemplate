using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CurrencyPublicScheme : PublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite sprite;

        public override string ID => id;
        public Sprite Sprite => sprite;
    }
}
