using Data.Scheme.Public;

namespace Services.TutorialService
{
    public interface ITutorialService
    {
        void Initialize();
        void StopTutorial();
        void StartTutorial(TutorialType startedTutorial);
    }
}