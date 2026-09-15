using Data;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Services.TutorialService
{
    public interface ITutorialService
    {
        void Initialize();
        void StopTutorial();
        void StartTutorial(TutorialId startedTutorial);
    }
}
