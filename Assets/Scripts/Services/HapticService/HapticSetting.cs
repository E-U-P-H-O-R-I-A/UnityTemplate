using System;
using UnityEngine;

namespace Services.HapticService
{
    [Serializable]
    public struct HapticSetting
    {
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        [SerializeField] private float duration;

        public float Duration => duration;
        public float Amplitude => amplitude;
        public float Frequency => frequency;
    }
}
