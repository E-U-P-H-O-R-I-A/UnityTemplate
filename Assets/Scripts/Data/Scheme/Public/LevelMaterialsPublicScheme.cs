using System;
using UnityEngine;

namespace Data.Scheme.Public
{
    [Serializable]
    public class LevelMaterialsPublicScheme : BasePublicScheme
    {
        [SerializeField] private int id;
        [SerializeField] private Material material;

        public Material Material => material;

        public override int ID => id;
        public override string StringID => $"ID: {ID} Name: {material?.name}";
    }
}