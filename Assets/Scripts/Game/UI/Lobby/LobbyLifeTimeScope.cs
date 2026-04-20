using Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Lobby
{
    public class LobbyLifeTimeScope : SceneLifetimeScope
    {
        [Space]
        [SerializeField] private Lobby lobby;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(lobby);
        }
    }
}
