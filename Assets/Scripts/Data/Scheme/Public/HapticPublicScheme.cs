using System;
using System.Collections.Generic;
using Services.HapticService;
using UnityEngine;

namespace Data.Scheme.Public
{
    public enum HapticType
    {
        Selection = 0,
        Success = 1,
        Warning = 2,
        Failure = 3,
        LightImpact = 4,
        MediumImpact = 5,
        HeavyImpact = 6,
        RigidImpact = 7,
        SoftImpact = 8,
    }

    [Serializable]
    public class HapticPublicScheme : BasePublicScheme
    {
        [SerializeField] private HapticType type;
        [SerializeField] private HapticSequence sequence;
        
        public HapticType Type => type;
        public HapticSequence Sequence => sequence;
        public override string ID => type.ToString();
    }
}