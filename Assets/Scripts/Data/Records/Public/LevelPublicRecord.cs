using System;
using UnityEngine;
using Tools.LevelEditor;

namespace Data
{
    [Serializable]
    public class LevelPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private TextAsset json;

        public override string Id => id;
        public TextAsset Json => json;

        public LevelSaveData GetSaveData() =>
            json != null ? JsonUtility.FromJson<LevelSaveData>(json.text) : null;
    }
}
