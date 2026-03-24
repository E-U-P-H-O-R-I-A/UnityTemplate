using System;
using UnityEngine;

namespace Utility.LevelEditor
{
    [Serializable]
    public class LevelElementSaveData
    {
        public int ElementID;
        public int MaterialID;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}