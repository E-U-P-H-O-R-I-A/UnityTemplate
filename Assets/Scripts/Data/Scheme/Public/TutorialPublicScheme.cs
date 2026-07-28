using System;
using System.Collections.Generic;
using Data.Scheme.Public;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    public enum TutorialType
    {
        None = 0,
    }

    [Serializable]
    public class TutorialPublicScheme : PublicScheme
    {
        [SerializeField] private TutorialType type;
        
        [Space(5), ListDrawerSettings(Expanded = true)] 
        [SerializeReference] private List<TutorialStep> steps;

        public IReadOnlyList<TutorialStep> Steps => steps;

        public override string ID => type.ToString();
    }
}