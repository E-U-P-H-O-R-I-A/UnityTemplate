using System;
using UnityEngine;
using Tools.LevelEditor;

namespace Data
{
    [Serializable]
    public class LevelElementPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private LevelElement prefab;

        public override string Id => id;
        public LevelElement Prefab => prefab;

        public LevelElementType LevelElementType =>
            prefab != null ? prefab.LevelElementType : LevelElementType.Wall;
    }
}
