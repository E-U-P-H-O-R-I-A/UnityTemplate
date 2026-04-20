using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.Model.Public;
using Data.Scheme.Public;
using Services.LogService;
using Services.PublicModelProvider;
using Services.WindowsService.Windows;
using UnityEngine;
using Utility.Factory;
using VContainer;

namespace Services.WindowsService
{
    public class WindowService : MonoBehaviour, IWindowService
    {
        [SerializeField] private Canvas canvas;
            
        private readonly Dictionary<WindowType, BaseWindow> windows = new();
        private readonly Stack<WindowRequest> windowHistory = new();
        private readonly List<WindowRequest> queue = new();

        private IPublicModelProvider publicModelProvider;
        private IFactory factory;
        private ILogService logService;

        private WindowsPublicModel publicModel;
        private BaseWindow currentWindow;
        
        private bool isProcessingOpenRequest;

        [Inject]
        public void Construct(IPublicModelProvider publicModelProvider, IFactory factory, ILogService logService)
        {
            this.publicModelProvider = publicModelProvider;
            this.factory = factory;
            this.logService = logService;
        }

        public void Awake() => 
            DontDestroyOnLoad(this);

        public void Initialize() => 
            publicModel = publicModelProvider.GetModel<WindowsPublicModel>();

        public void OpenWindow(WindowType type, BaseWindowParams @params)
        {
            var request = CreateRequest(type, @params);
            EnqueueRequest(request);
        }

        public void OpenSubWindow(WindowType type, BaseWindowParams @params = null)
        {
            var request = CreateRequest(type, @params);

            if (currentWindow == null)
            {
                OpenWindow(type, @params);
                return;
            }

            if (@params?.IsHidePrevious == true)
                currentWindow.ForceHide();
            
            OpenRequestAsync(request).Forget();
        }
        
        private void ClearHistory() => 
            windowHistory.Clear();

        private BaseWindow GetWindow(WindowType type) => 
            windows.TryGetValue(type, out BaseWindow window) ? window : CreateWindow(type);

        private WindowPublicScheme GetWindowScheme(WindowType type) => 
            publicModel.GetScheme(type.ToString());

        private WindowRequest CreateRequest(WindowType type, BaseWindowParams @params)
        {
            var publicScheme = GetWindowScheme(type);
            return new WindowRequest(type, @params, publicScheme.Priority);
        }

        private BaseWindow CreateWindow(WindowType type)
        {
            var publicScheme = GetWindowScheme(type);
            var window = factory.CreateFromPrefab(publicScheme.Prefab, canvas.transform);
            window.Closed += OnWindowClosed;
            
            windows[type] = window;
            return window;
        }
        
        private void EnqueueRequest(WindowRequest request)
        {
            var insertIndex = queue.Count;

            for (var i = 0; i < queue.Count; i++)
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
                ClearHistory();
                return;
            }
            
            WindowRequest request = queue.First();
            
            queue.Remove(request);

            ClearHistory();
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

                BaseWindow window = GetWindow(request.WindowType);

                currentWindow = window;

                await window.OpenAsync(request.Params);
                
                currentWindow.transform.SetAsLastSibling();
            }
            catch(Exception ex)
            {
                logService.LogError($"[{name}] Failed to open window request {request.WindowType}: {ex}", LogCategory.Windows);

                if (windowHistory.Count > 0)
                    windowHistory.Pop();

                if (windowHistory.Count > 0)
                {
                    WindowRequest previousRequest = windowHistory.Peek();
                    BaseWindow previousWindow = GetWindow(previousRequest.WindowType);

                    currentWindow = previousWindow;

                    if (request.Params?.IsHidePrevious == true || !previousWindow.gameObject.activeSelf)
                        previousWindow.ForceShow();

                    previousWindow.transform.SetAsLastSibling();
                    return;
                }

                currentWindow = null;
                ProcessQueue();
            }
            finally
            {
                isProcessingOpenRequest = false;
            }
        }
        
        private async void OnWindowClosed(BaseWindow closedWindow)
        {
            if (closedWindow != currentWindow)
                return;
            
            if (windowHistory.Count > 0)
            {
                windowHistory.Pop();

                if (windowHistory.Count > 0)
                {
                    WindowRequest previousRequest = windowHistory.Pop();
                    BaseWindow previousWindow = GetWindow(previousRequest.WindowType);

                    if (previousWindow == closedWindow)
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
