using Cysharp.Threading.Tasks;
using Data;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;

namespace Game.UI.Lobby.Tabs.Levels
{
    public class LevelsModel
    {
        private readonly IPrivateContainerProvider privateContainerProvider;
        private readonly LevelPublicContainer levelPublicContainer;
        private readonly LevelProgressPrivateRecord progress;

        public LevelsModel(IPrivateContainerProvider privateContainerProvider,
            IPublicContainerProvider publicContainerProvider)
        {
            this.privateContainerProvider = privateContainerProvider;

            levelPublicContainer = publicContainerProvider.GetContainer<LevelPublicContainer>();
            progress = privateContainerProvider.GetContainer<LevelProgressPrivateContainer>().GetRecord();
        }

        public int CurrentLevel => progress.CurrentLevel;
        public int CurrentLevelNumber => progress.CurrentLevel + 1;
        public bool IsLevelCompleted => progress.IsLevelCompleted;

        public LevelPublicRecord GetCurrentLevelRecord()
        {
            var records = levelPublicContainer.Records;

            return records.Count == 0 ? null : records[progress.CurrentLevel % records.Count];
        }

        public void CompleteLevel()
        {
            progress.Complete();
            Save();
        }

        public void ConsumeCompletedFlag()
        {
            if (!progress.IsLevelCompleted)
                return;

            progress.IsLevelCompleted = false;
            Save();
        }

        private void Save() =>
            privateContainerProvider.SaveContainer<LevelProgressPrivateContainer>().Forget();
    }
}
