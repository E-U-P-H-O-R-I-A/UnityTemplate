using System;
using System.Collections.Generic;

namespace Utility.LevelEditor
{
    [Serializable]
    public class LevelSaveData
    {
        public List<LevelElementSaveData> Elements = new();
    }
}