using System;
using UnityEngine;
using Utility.LevelEditor;

namespace Data.Scheme.Public
{
    public enum LevelElementType
    {
        Wall = 1,
        Floor = 2,
        Decor = 3,
    }
    
    [Serializable]
    public class LevelElementPublicScheme : BasePublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private LevelElement prefab;

        public LevelElement Prefab => prefab;
        public LevelElementType LevelElementType => prefab != null ? prefab.LevelElementType : LevelElementType.Wall;

        public override string ID => id;
    }
}