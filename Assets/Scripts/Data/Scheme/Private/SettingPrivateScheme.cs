using System;
using UnityEngine;

namespace Data.Scheme.Private
{
    [Serializable]
    public class SettingPrivateScheme : BasePrivateScheme
    {
        [SerializeField] private bool hapticStatus = true;
        
        public override string ID => "Settings";

        public bool Haptic
        {
            get => hapticStatus;
            set => hapticStatus = value;
        }
    }
}