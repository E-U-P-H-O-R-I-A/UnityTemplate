using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.WindowsService.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Services.WindowsService.Windows
{
    public class BaseWindowParams
    {
        public bool IsHidePrevious = false;
    }

    public abstract class BaseWindow : MonoBehaviour
    {
        public event Action<BaseWindow> Closed;
        
        public abstract WindowType Type { get; }
        public abstract UniTask CloseAsync();
        public abstract UniTask OpenAsync(object windowParams);

        public void ForceHide() => 
            gameObject.SetActive(false);

        public void ForceShow() => 
            gameObject.SetActive(true);

        protected void RaiseClosed() => 
            Closed?.Invoke(this);
    }

    public abstract class BaseWindow<TParams> : BaseWindow where TParams : BaseWindowParams
    {
        [Header("Base Window")]
        [SerializeField] private List<Button> buttonClose;
        [SerializeField] private List<BaseWindowAnimation> windowAnimations;

        private bool isProcessingOpening;
        private bool isProcessingClosing;

        protected TParams Params { get; private set; }

        protected virtual void Awake() => 
            ForceHide();

        public override async UniTask OpenAsync(object @params = null)
        {
            if (isProcessingOpening)
                return;

            if (isProcessingClosing)
                return;
            
            isProcessingOpening = true;
            Params = @params as TParams;

            try
            {
                KillAllAnimations();
                
                await OnBeforeOpen(Params);

                ForceShow();

                await PlayOpenAnimationsAsync();

                await OnAfterOpened(Params);

                DisableButtons();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
                DisableButtons();
                ForceHide();
            }
            finally
            {
                isProcessingOpening = false;
            }
        }

        public override async UniTask CloseAsync()
        {
            if (isProcessingClosing)
                return;

            if (isProcessingOpening)
                return;

            isProcessingClosing = true;
            DisableButtons();

            try
            {
                await OnBeforeClose();

                KillAllAnimations();
                await PlayCloseAnimationsAsync();

                await OnAfterClosed();

                ForceHide();
                RaiseClosed();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
                ForceHide();
                RaiseClosed();
            }
            finally
            {
                isProcessingClosing = false;
            }
        }

        protected virtual UniTask OnBeforeOpen(TParams payload) => UniTask.CompletedTask;
        protected virtual UniTask OnAfterOpened(TParams payload) => UniTask.CompletedTask;
        protected virtual UniTask OnBeforeClose() => UniTask.CompletedTask;
        protected virtual UniTask OnAfterClosed() => UniTask.CompletedTask;

        private void OnCloseClick() => 
            CloseAsync().Forget();

        private void EnableButtons() => 
            buttonClose.ForEach(button => button.onClick.AddListener(OnCloseClick));

        private void DisableButtons() => 
            buttonClose.ForEach(button => button.onClick.RemoveAllListeners());
        
        private void KillAllAnimations()
        {
            if (windowAnimations == null || windowAnimations.Count == 0)
                return;

            foreach (var animation in windowAnimations)
            {
                if (animation == null)
                    continue;

                animation.Kill();
            }
        }
        
        private async UniTask PlayOpenAnimationsAsync()
        {
            if (windowAnimations == null || windowAnimations.Count == 0)
                return;

            var tasks = new List<UniTask>();

            foreach (var animation in windowAnimations)
            {
                if (animation == null)
                    continue;

                tasks.Add(animation.PlayOpenAsync());
            }

            if (tasks.Count > 0)
                await UniTask.WhenAll(tasks);
        }
        
        private async UniTask PlayCloseAnimationsAsync()
        {
            if (windowAnimations == null || windowAnimations.Count == 0)
                return;

            var tasks = new List<UniTask>();

            foreach (var animation in windowAnimations)
            {
                if (animation == null)
                    continue;

                tasks.Add(animation.PlayCloseAsync());
            }

            if (tasks.Count > 0)
                await UniTask.WhenAll(tasks);
        }
        
#if UNITY_EDITOR
        [PropertySpace]
        [Button("Find All Window Animations")]
        private void FindAllWindowAnimations()
        {
            windowAnimations = GetComponentsInChildren<BaseWindowAnimation>(true)
                .Distinct()
                .ToList();

            UnityEditor.EditorUtility.SetDirty(this);

            Debug.Log($"[{name}] Found {windowAnimations.Count} BaseWindowAnimation components.", this);
        }

#endif

    }
}
