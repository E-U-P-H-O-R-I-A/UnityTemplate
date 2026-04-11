using System;
using UnityEngine;

namespace Utility.LevelEditor
{
    [Serializable]
    public class LevelElementSaveData
    {
        public string ElementID;
        public string MaterialID;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}
