using System;
using VContainer.Unity;

namespace Game.UI.Lobby
{
    public class LobbyEntryPoint : IStartable, IDisposable
    {
        private readonly LobbyController lobbyController;

        public LobbyEntryPoint(LobbyController lobbyController)
        {
            this.lobbyController = lobbyController;
        }

        public void Start() =>
            lobbyController.Initialize();

        public void Dispose() =>
            lobbyController.Dispose();
    }
}
