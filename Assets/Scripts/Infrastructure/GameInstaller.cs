using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetManagement;
using Services.AssetProvider;
using Services.LogService;
using Services.SceneProvider;
using UnityEngine;
using Utility.CoroutineRunner;
using Utility.Factory;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindServices();
            BindInfrastructures();
        }

        private void BindInfrastructures()
        {
            Container.Bind<GameStateMachine>().AsSingle().NonLazy();
            
            Container.BindFactory<GameBootstrapper, GameBootstrapper.Factory>()
                .FromComponentInNewPrefabResource(AssetsPath.GAMEBOOTSTRAPPER).AsSingle();
            
            Container.Bind<ICoroutineRunner>().To<CoroutineRunner>()
                .FromComponentInNewPrefabResource(AssetsPath.COROUTINE_RUNNER).AsSingle();
        }
        
        private void BindServices()
        {
            Container.Bind<Utility.Factory.IFactory>().To<Factory>().AsSingle();
            Container.Bind<IAssetsProvider>().To<AssetsProvider>().AsSingle();
            Container.Bind<ISceneProvider>().To<SceneProvider>().AsSingle();
            Container.Bind<ILogService>().To<LogService>().AsSingle();
        }
    }
}