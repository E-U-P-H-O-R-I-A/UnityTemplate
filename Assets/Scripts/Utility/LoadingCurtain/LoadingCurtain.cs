using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Utility.LoadingCurtain
{
    public class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Slider progressFill;

        private float current;

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            canvasGroup.alpha = 1f;
            progressFill.value = 0f;
            current = 0f;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
        }

        public async UniTask Finish()
        {
            while (current < 1f)
            {
                current = Mathf.MoveTowards(current, 1f, Time.deltaTime * 3f);
                progressFill.value = current;

                await UniTask.Yield();
            }

            await UniTask.WaitForEndOfFrame();
        }

        public async UniTask AnimatePhase(UniTask task, float target)
        {
            target = Mathf.Clamp01(target);

            while (!task.Status.IsCompleted())
            {
                current = Mathf.MoveTowards(current, target, Time.deltaTime);
                progressFill.value = current;

                await UniTask.Yield();
            }
            
            current = target;
            progressFill.value = current;

            await task;
        }
    }
}