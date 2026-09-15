using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Services.LogService;
using Services.PublicContainerProvider;
using Services.WindowsService.Factory;
using Services.WindowsService.Windows;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService
{
    public class WindowService : IWindowService, IDisposable
    {
        private readonly Dictionary<WindowType, Window> windows = new();
        private readonly Stack<WindowRequest> windowHistory = new();
        private readonly List<WindowRequest> queue = new();

        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly IWindowFactory windowFactory;
        private readonly WindowsRootView rootView;
        private readonly ILogService logService;

        private WindowPublicContainer publicContainer;
        private IWindowController currentWindow;

        private bool isProcessingOpenRequest;

        public WindowService(WindowsRootView rootView, IWindowFactory windowFactory,
            IPublicContainerProvider publicContainerProvider, ILogService logService)
        {
            this.publicContainerProvider = publicContainerProvider;
            this.windowFactory = windowFactory;
            this.logService = logService;
            this.rootView = rootView;
        }

        public void Initialize() =>
            publicContainer = publicContainerProvider.GetContainer<WindowPublicContainer>();

        public void OpenWindow(WindowType type, WindowParams windowParams = null) =>
            EnqueueRequest(CreateRequest(type, windowParams));

        public void OpenSubWindow(WindowType type, WindowParams windowParams = null)
        {
            if (currentWindow == null)
            {
                OpenWindow(type, windowParams);
                return;
            }

            if (windowParams?.IsHidePrevious == true)
                currentWindow.ForceHide();

            OpenRequestAsync(CreateRequest(type, windowParams)).Forget();
        }

        public void Dispose()
        {
            foreach (Window window in windows.Values)
            {
                window.Controller.Closed -= OnWindowClosed;
                window.Dispose();
            }

            windows.Clear();
            windowHistory.Clear();
            queue.Clear();

            currentWindow = null;
        }

        private WindowRequest CreateRequest(WindowType type, WindowParams windowParams)
        {
            WindowPublicRecord publicRecord = publicContainer.GetRecord(type);

            return new WindowRequest(type, windowParams, publicRecord.Priority);
        }

        private IWindowController GetWindow(WindowType type)
        {
            if (windows.TryGetValue(type, out Window window))
                return window.Controller;

            window = windowFactory.Create(type, rootView.Container);
            window.Controller.Closed += OnWindowClosed;

            windows[type] = window;

            return window.Controller;
        }

        private void EnqueueRequest(WindowRequest request)
        {
            int insertIndex = queue.Count;

            for (int i = 0; i < queue.Count; i++)
            {
                if (request.Priority > queue[i].Priority)
                {
                    insertIndex = i;
                    break;
                }
            }

            queue.Insert(insertIndex, request);

            ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (currentWindow != null)
                return;

            if (queue.Count == 0)
            {
                windowHistory.Clear();
                return;
            }

            WindowRequest request = queue[0];

            queue.RemoveAt(0);
            windowHistory.Clear();

            OpenRequestAsync(request).Forget();
        }

        private async UniTask OpenRequestAsync(WindowRequest request)
        {
            while (isProcessingOpenRequest)
                await UniTask.Yield();

            isProcessingOpenRequest = true;

            try
            {
                windowHistory.Push(request);

                IWindowController window = GetWindow(request.WindowType);

                currentWindow = window;

                await window.Open(request.Params);

                window.BringToFront();
            }
            catch (Exception exception)
            {
                logService.LogError($"[WindowService] Failed to open window {request.WindowType}: {exception}", LogCategory.Windows);

                Rollback(request);
            }
            finally
            {
                isProcessingOpenRequest = false;
            }
        }

        private void Rollback(WindowRequest failedRequest)
        {
            if (windowHistory.Count > 0)
                windowHistory.Pop();

            if (windowHistory.Count > 0)
            {
                WindowRequest previousRequest = windowHistory.Peek();
                IWindowController previousWindow = GetWindow(previousRequest.WindowType);

                currentWindow = previousWindow;

                if (failedRequest.Params?.IsHidePrevious == true || !previousWindow.IsVisible)
                    previousWindow.ForceShow();

                previousWindow.BringToFront();
                return;
            }

            currentWindow = null;

            ProcessQueue();
        }

        private void OnWindowClosed(IWindowController closedWindow) =>
            HandleWindowClosed(closedWindow).Forget();

        private async UniTaskVoid HandleWindowClosed(IWindowController closedWindow)
        {
            if (closedWindow != currentWindow)
                return;

            if (windowHistory.Count > 0)
            {
                windowHistory.Pop();

                if (windowHistory.Count > 0)
                {
                    WindowRequest previousRequest = windowHistory.Pop();

                    await UniTask.Yield();
                    await OpenRequestAsync(previousRequest);
                    return;
                }
            }

            await UniTask.Yield();

            currentWindow = null;

            ProcessQueue();
        }
    }
}
