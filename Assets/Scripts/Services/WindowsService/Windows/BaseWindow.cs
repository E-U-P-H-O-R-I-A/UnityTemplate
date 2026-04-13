using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.WindowsService.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Services.WindowsService.Windows
{
    public abstract class BaseWindowParams
    {
        public bool IsHideWindow = true;
    }

    public abstract class BaseWindow : MonoBehaviour
    {
        public event Action<BaseWindow> Closed;
        
        public abstract WindowType Type { get; }
        public abstract UniTask CloseAsync();
        public abstract UniTask OpenAsync(object windowParams);

        protected void ForceHide() => 
            gameObject.SetActive(false);

        protected void ForceShow() => 
            gameObject.SetActive(true);

        protected void RaiseClosed() => 
            Closed?.Invoke(this);
    }

    public abstract class BaseWindow<TParams> : BaseWindow where TParams : BaseWindowParams
    {
        [Header("Base Window")]
        [SerializeField] private List<Button> buttonClose;
        [SerializeField] private BaseWindowAnimation windowAnimation;

        private bool isProcessingOpening;
        private bool isProcessingClosing;

        protected TParams Params { get; private set; }

        protected virtual void Awake() => 
            ForceHide();

        public override async UniTask OpenAsync(object @params = null)
        {
            Params = @params as TParams;

            try
            {
                windowAnimation?.Kill();
                
                await OnBeforeOpen(Params);

                ForceShow();

                if (windowAnimation != null)
                    await windowAnimation.PlayOpenAsync();

                await OnAfterOpened(Params);

                EnableButtons();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
                DisableButtons();
                ForceHide();
            }
        }

        public override async UniTask CloseAsync()
        {
            DisableButtons();

            try
            {
                await OnBeforeClose();

                if (windowAnimation != null)
                {
                    windowAnimation.Kill();
                    await windowAnimation.PlayCloseAsync();
                }

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
            buttonClose.ForEach(button => button.onClick.RemoveListener(OnCloseClick));
    }
}
