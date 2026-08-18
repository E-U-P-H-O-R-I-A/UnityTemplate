using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelMaterialsPublicScheme : PublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private Material material;

        public override string ID => id;
        public Material Material => material;
    }
}
