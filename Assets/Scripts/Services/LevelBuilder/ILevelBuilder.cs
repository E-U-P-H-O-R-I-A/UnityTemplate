using Data;
using Tools.LevelEditor;
using UnityEngine;

namespace Services.LevelBuilder
{
    public interface ILevelBuilder
    {
        Level CurrentLevel { get; }

        Level Build(LevelPublicRecord record, Transform parent = null);

        void Clear();
    }
}
