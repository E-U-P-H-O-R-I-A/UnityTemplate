using System;
using System.Collections.Generic;
using Services.TutorialService;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class TutorialPublicRecord : PublicRecord
    {
        [SerializeField] private string id;
        [SerializeReference] private List<TutorialStep> steps = new();

        public override string Id => id;
        public IReadOnlyList<TutorialStep> Steps => steps;
    }
}
