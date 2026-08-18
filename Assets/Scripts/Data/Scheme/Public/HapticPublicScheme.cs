using System;
using Services.HapticService;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class HapticPublicScheme : PublicScheme
    {
        [SerializeField] private string id;
        [SerializeField] private HapticSequence sequence;

        public override string ID => id;
        public HapticSequence Sequence => sequence;
    }
}
