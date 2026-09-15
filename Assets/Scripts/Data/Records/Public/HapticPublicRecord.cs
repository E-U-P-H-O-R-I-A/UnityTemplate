using System;
using Services.HapticService;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class HapticPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeField] private HapticSequence sequence;

        public override string Id => id;
        public HapticSequence Sequence => sequence;
    }
}
