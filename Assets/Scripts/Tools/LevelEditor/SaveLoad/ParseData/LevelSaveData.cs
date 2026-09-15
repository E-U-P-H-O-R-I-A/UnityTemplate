using System;
using System.Collections.Generic;

namespace Tools.LevelEditor
{
    [Serializable]
    public class LevelSaveData
    {
        public List<LevelElementSaveData> Elements = new();
    }
}