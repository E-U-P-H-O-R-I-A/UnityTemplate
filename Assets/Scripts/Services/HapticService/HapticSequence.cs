using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.HapticService
{
    [Serializable]
    public struct HapticSequence
    {
        [SerializeField] private List<HapticSetting> settings;
        
        public List<HapticSetting> Sequence => settings;
    }
}