using DefaultNamespace;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private TestPublicModel testPublicModel;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(testPublicModel);
        }
    }
}