using System;
using UnityEngine;

namespace Data.Scheme.Public
{
    [Serializable]
    public class CurrencyPublicScheme : BasePublicScheme
    {
        [SerializeField] private Sprite sprite;

        public Sprite Sprite => sprite;
    }
}