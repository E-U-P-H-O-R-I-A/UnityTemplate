using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CurrencyPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite sprite;

        public override string Id => id;
        public Sprite Sprite => sprite;
    }
}
