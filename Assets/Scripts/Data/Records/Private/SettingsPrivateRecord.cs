using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SettingsPrivateRecord : PrivateRecord
    {
        private const string SETTINGS = "Settings";
        
        [SerializeField] private bool hapticEnabled = true;
        
        public override string Id => SETTINGS;

        public bool HapticEnabled
        {
            get => hapticEnabled;
            set => hapticEnabled = value;
        }
    }
}
