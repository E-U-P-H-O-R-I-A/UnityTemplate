using System;
using Data;
using Services.LogService;
using Services.PublicContainerProvider;
using Services.WindowsService.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService.Factory
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly IObjectResolver resolver;
        private readonly ILogService logService;

        public WindowFactory(IObjectResolver resolver, IPublicContainerProvider publicContainerProvider, ILogService logService)
        {
            this.publicContainerProvider = publicContainerProvider;
            this.resolver = resolver;
            this.logService = logService;
        }

        public Window Create(WindowType type, Transform parent)
        {
            WindowPublicRecord record = publicContainerProvider
                .GetContainer<WindowPublicContainer>()
                .GetRecord(type);

            if (record == null)
                throw new InvalidOperationException($"[WindowFactory] Window {type} has no record in {nameof(WindowPublicContainer)}");

            if (record.Prefab == null)
                throw new InvalidOperationException($"[WindowFactory] Window {type} has no prefab assigned");

            WindowView view = resolver.Instantiate(record.Prefab, parent);
            Type controllerType = view.ControllerType;

            IScopedObjectResolver scope = resolver.CreateScope(builder =>
            {
                builder.RegisterInstance(view).AsSelf().As<WindowView>();
                builder.Register(controllerType, Lifetime.Scoped).As<IWindowController>();
            });

            IWindowController controller = scope.Resolve<IWindowController>();
            controller.Initialize();

            logService.Log($"[WindowFactory] Created window {type} with {controllerType.Name}", LogCategory.Windows);

            return new Window(type, view, controller, scope);
        }
    }
}
