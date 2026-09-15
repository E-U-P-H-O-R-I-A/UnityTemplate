using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services.WindowsService.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Services.WindowsService.Windows
{
    public abstract class WindowView : View
    {
        [Header("Base Window")]
        [SerializeField] private List<Button> buttonClose;
        [SerializeField] private List<BaseWindowAnimation> windowAnimations;

        public event Action CloseRequested;

        public abstract Type ControllerType { get; }

        public void BringToFront() =>
            transform.SetAsLastSibling();

        public void SetCloseInteractable(bool value)
        {
            foreach (Button button in buttonClose)
            {
                if (button != null)
                    button.interactable = value;
            }
        }

        public async UniTask PlayOpenAsync()
        {
            KillAnimations();

            Show();

            await PlayAnimationsAsync(animation => animation.PlayOpenAsync());
        }

        public async UniTask PlayCloseAsync()
        {
            KillAnimations();

            await PlayAnimationsAsync(animation => animation.PlayCloseAsync());

            Hide();
        }

        protected virtual void Awake()
        {
            Hide();

            foreach (Button button in buttonClose)
            {
                if (button != null)
                    button.onClick.AddListener(OnCloseClicked);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (Button button in buttonClose)
            {
                if (button != null)
                    button.onClick.RemoveListener(OnCloseClicked);
            }
        }

        private void OnCloseClicked() =>
            CloseRequested?.Invoke();

        private void KillAnimations()
        {
            foreach (BaseWindowAnimation animation in windowAnimations)
            {
                if (animation != null)
                    animation.Kill();
            }
        }

        private async UniTask PlayAnimationsAsync(Func<BaseWindowAnimation, UniTask> play)
        {
            if (windowAnimations == null || windowAnimations.Count == 0)
                return;

            List<UniTask> tasks = new(windowAnimations.Count);

            foreach (BaseWindowAnimation animation in windowAnimations)
            {
                if (animation != null)
                    tasks.Add(play(animation));
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
        }
#endif
    }

    public abstract class WindowView<TController> : WindowView where TController : IWindowController
    {
        public sealed override Type ControllerType => typeof(TController);
    }
}
