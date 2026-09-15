using Data;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Services.TutorialService
{
    public interface ITutorialService : IInitializableService
    {
        void StopTutorial();
        void StartTutorial(TutorialId startedTutorial);
    }
}
