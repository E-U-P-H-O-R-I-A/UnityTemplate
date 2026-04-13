using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.Model.Public;
using Data.Scheme.Public;
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

        private WindowsPublicModel publicModel;
        private BaseWindow currentWindow;
        
        [Inject]
        public void Construct(IPublicModelProvider publicModelProvider, IFactory factory)
        {
            this.publicModelProvider = publicModelProvider;
            this.factory = factory;
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
            
            OpenRequestAsync(request);
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
            OpenRequestAsync(request);
        }

        private async UniTask OpenRequestAsync(WindowRequest request)
        {
            try
            {
                windowHistory.Push(request);

                BaseWindow window = GetWindow(request.WindowType);

                currentWindow = window;

                await window.OpenAsync(request.Params);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex, this);
                currentWindow = null;
                ProcessQueue();
            }
        }
        
        private async void OnWindowClosed(BaseWindow closedWindow)
        {
            if (closedWindow != currentWindow)
                return;

            if (windowHistory.Count > 0)
                windowHistory.Pop();

            if (windowHistory.Count > 0)
            {
                WindowRequest previousWindow = windowHistory.Pop();
                await OpenRequestAsync(previousWindow);
                return;
            }

            currentWindow = null;

            ProcessQueue();
        }
    }
}
