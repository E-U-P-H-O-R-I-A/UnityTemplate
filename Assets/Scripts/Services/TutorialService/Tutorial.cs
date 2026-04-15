using Data.Scheme.Private;
using Data.Scheme.Public;

namespace Services.TutorialService
{
    public class Tutorial
    {
        public int StepIndex;
        public TutorialType Type;

        public TutorialPublicScheme PublicScheme;
        public TutorialPrivateScheme PrivateScheme;

        public bool IsRunning => Type is not TutorialType.None;

        public Tutorial()
        {
            Clear();
        }

        public void Clear()
        {
            StepIndex = -1;
            Type = TutorialType.None;

            PublicScheme = null;
            PrivateScheme = null;
        }

        public override string ToString() =>
            $"Type={Type}, StepIndex={StepIndex}";
    }
}