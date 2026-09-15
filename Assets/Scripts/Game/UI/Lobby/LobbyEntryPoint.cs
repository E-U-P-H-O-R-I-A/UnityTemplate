using VContainer.Unity;

namespace Game.UI.Lobby
{
    public class LobbyEntryPoint : IStartable
    {
        private readonly Lobby lobby;

        public LobbyEntryPoint(Lobby lobby)
        {
            this.lobby = lobby;
        }

        public void Start() =>
            lobby.Initialize();
    }
}
