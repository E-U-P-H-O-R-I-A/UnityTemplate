using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelProgressPrivateRecord : PrivateRecord
    {
        private const string LEVEL_PROGRESS = "LevelProgress";

        [SerializeField] private int currentLevel;
        [SerializeField] private bool isLevelCompleted;

        public override string Id => LEVEL_PROGRESS;

        public int CurrentLevel => currentLevel;

        public bool IsLevelCompleted
        {
            get => isLevelCompleted;
            set => isLevelCompleted = value;
        }

        public void Complete()
        {
            currentLevel++;
            isLevelCompleted = true;
        }
    }
}
