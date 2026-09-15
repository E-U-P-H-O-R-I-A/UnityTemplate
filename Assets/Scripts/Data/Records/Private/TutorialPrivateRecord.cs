using System;
using UnityEngine;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Data
{
    [Serializable]
    public class TutorialPrivateRecord : PrivateRecord
    {
        [RecordId] private TutorialId id;
        
        [SerializeField] private bool isComplete;

        public bool IsComplete => isComplete;
        public override string Id => id.ToString();

        public TutorialPrivateRecord(TutorialId id)
        {
            this.id = id;
            isComplete = false;
        }

        public void Complete()
        {
            isComplete = true;
        }
    }
}
