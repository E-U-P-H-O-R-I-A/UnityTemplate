using System;
using UnityEngine;
using TutorialType = Data.TutorialPublicModel.Type;

namespace Data
{
    [Serializable]
    public class TutorialPrivateScheme : PrivateScheme
    {
        [SchemeId] private TutorialType type;
        
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
