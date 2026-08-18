using System;

namespace Services.TutorialService
{
    [Serializable]
    public abstract class TutorialStep
    {
        public event Action<TutorialStep> Completed;

        public virtual void StartStep() => 
            Complete();

        protected virtual void Complete() => 
            Completed?.Invoke(this);
    }
}