using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SettingPrivateScheme : PrivateScheme
    {
        private const string SETTINGS = "Settings";
        
        [SerializeField] private bool hapticStatus = true;
        
        public override string ID => SETTINGS;

        public bool Haptic
        {
            get => hapticStatus;
            set => hapticStatus = value;
        }
    }
}