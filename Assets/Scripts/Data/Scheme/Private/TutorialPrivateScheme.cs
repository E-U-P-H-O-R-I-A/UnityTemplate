using System;
using Data.Scheme.Public;
using UnityEngine;

namespace Data.Scheme.Private
{
    [Serializable]
    public class TutorialPrivateScheme : BasePrivateScheme
    {
        [SerializeField] private TutorialType type;
        [SerializeField] private bool isComplete;

        public TutorialType Type => type;
        public bool IsComplete => isComplete;
        public override string ID => type.ToString();

        public TutorialPrivateScheme(TutorialType type)
        {
            this.type = type;
            isComplete = false;
        }

        public void CompleteTutorial()
        {
            isComplete = true;
        }
    }
}