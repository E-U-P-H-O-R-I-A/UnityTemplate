using System;
using Services.WindowsService.Windows;
using UnityEngine;

namespace Data.Scheme.Public
{
    public enum WindowType
    {
        Shop = 0,
        NoAds = 1,
    }

    public enum WindowPriority
    {
        Low = 0,
        Normal = 100,
        High = 200,
        Critical = 1000
    }

    [Serializable]
    public class WindowPublicScheme : BasePublicScheme
    {
        [SerializeField] private WindowType type;
        [SerializeField] private WindowPriority priority;
        [SerializeField] private BaseWindow prefab;

        public WindowType Type => type;
        public BaseWindow Prefab => prefab;
        public int Priority => (int)priority;

        public override string ID => type.ToString();
    }
}