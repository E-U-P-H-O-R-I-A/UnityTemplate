using System;
using System.Collections.Generic;
using Services.TutorialService;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class TutorialPublicScheme : PublicScheme
    {
        [SerializeField] private string id;
        [SerializeReference] private List<TutorialStep> steps = new();

        public override string ID => id;
        public IReadOnlyList<TutorialStep> Steps => steps;
    }
}
