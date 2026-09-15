using System;
using Signals;
using UnityEngine;
using WindowType = Data.WindowPublicContainer.Id;

namespace Game.Gameplay
{
    [Serializable]
    public class GameplayWindowsSettings
    {
        [SerializeField] private WindowType winWindow;
        [SerializeField] private WindowType loseWindow;

        public WindowType GetResultWindow(LevelResult result) =>
            result == LevelResult.Win ? winWindow : loseWindow;
    }
}
