using Data;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Services.TutorialService
{
    public class Tutorial
    {
        public int StepIndex;
        public TutorialId Type;

        public TutorialPublicRecord PublicRecord;
        public TutorialPrivateRecord PrivateRecord;

        public bool IsRunning => Type is not TutorialId.None;

        public Tutorial()
        {
            Clear();
        }

        public void Clear()
        {
            StepIndex = -1;
            Type = TutorialId.None;

            PublicRecord = null;
            PrivateRecord = null;
        }

        public override string ToString() =>
            $"Type={Type}, StepIndex={StepIndex}";
    }
}
