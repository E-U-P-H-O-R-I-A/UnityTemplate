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
        [SerializeField] private BaseWindow prefab;

        public override string Id => id;
        public BaseWindow Prefab => prefab;
        public int Priority => (int)priority;
    }
}
