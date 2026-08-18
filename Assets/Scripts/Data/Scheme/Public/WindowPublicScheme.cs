using System;
using Services.WindowsService.Windows;
using UnityEngine;

namespace Data
{
    public enum WindowPriority
    {
        Low = 0,
        Normal = 100,
        High = 200,
        Critical = 1000
    }

    [Serializable]
    public class WindowPublicScheme : PublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private WindowPriority priority;
        [SerializeField] private BaseWindow prefab;

        public override string ID => id;
        public BaseWindow Prefab => prefab;
        public int Priority => (int)priority;
    }
}
