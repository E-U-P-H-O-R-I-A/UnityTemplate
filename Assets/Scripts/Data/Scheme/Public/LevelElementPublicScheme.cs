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
        [SerializeField] private int id;
        [SerializeField] private LevelElement prefab;

        public LevelElement Prefab => prefab;
        public LevelElementType LevelElementType => prefab != null ? prefab.LevelElementType : LevelElementType.Wall;

        public override int ID => (int)LevelElementType * 10000 + id;
        public override string StringID => $"ID: {ID} Name: {(prefab != null ? prefab.name : "")}";
    }
}