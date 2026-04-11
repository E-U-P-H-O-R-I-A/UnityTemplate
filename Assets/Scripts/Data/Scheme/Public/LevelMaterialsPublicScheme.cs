using System;
using UnityEngine;

namespace Data.Scheme.Public
{
    [Serializable]
    public class LevelMaterialsPublicScheme : BasePublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private Material material;

        public Material Material => material;

        public override string ID => id;
    }
}
