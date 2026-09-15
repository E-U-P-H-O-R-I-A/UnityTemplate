using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelMaterialPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private Material material;

        public override string Id => id;
        public Material Material => material;
    }
}
