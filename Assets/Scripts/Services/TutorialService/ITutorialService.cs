using Data;
using TutorialType = Data.TutorialPublicModel.Type;

namespace Services.TutorialService
{
    public interface ITutorialService
    {
        void Initialize();
        void StopTutorial();
        void StartTutorial(TutorialType startedTutorial);
    }
}
