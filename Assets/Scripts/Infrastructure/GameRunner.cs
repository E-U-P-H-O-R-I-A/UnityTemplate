using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        private GameBootstrapper.Factory gameBootstrapperFactory;

        [Inject]
        void Construct(GameBootstrapper.Factory bootstrapperFactory) => 
            gameBootstrapperFactory = bootstrapperFactory;

        private void Awake()
        {
            var bootstrapper = FindObjectOfType<GameBootstrapper>();
      
            if(bootstrapper != null) return;

            gameBootstrapperFactory.Create();
        }
    }
}