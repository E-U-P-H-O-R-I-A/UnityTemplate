using System;
using Services.WindowsService;
using Services.WindowsService.Windows;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class WindowPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private WindowPriority priority;
        [SerializeField] private WindowView prefab;

        public override string Id => id;
        public WindowView Prefab => prefab;
        public int Priority => (int)priority;
    }
}
