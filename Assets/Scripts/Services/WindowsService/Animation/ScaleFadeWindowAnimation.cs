using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Services.WindowsService.Animation
{
    public sealed class ScaleFadeWindowAnimation : BaseWindowAnimation
    {
        [Header("Refs")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform contentRoot;

        [Header("Settings")]
        [SerializeField] private float openDuration = 0.25f;
        [SerializeField] private float closeDuration = 0.2f;
        [SerializeField] private float startScale = 0.9f;

        private Sequence currentSequence;

        public override void Kill()
        {
            if (currentSequence == null || !currentSequence.IsActive()) 
                return;
            
            currentSequence.Kill();
            currentSequence = null;
        }

        public override async UniTask PlayOpenAsync()
        {
            Kill();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (contentRoot != null)
            {
                contentRoot.localScale = Vector3.one * startScale;
            }

            currentSequence = DOTween.Sequence();
            currentSequence.SetUpdate(true);

            if (canvasGroup != null)
                currentSequence.Join(canvasGroup.DOFade(1f, openDuration));

            if (contentRoot != null)
                currentSequence.Join(contentRoot.DOScale(1f, openDuration).SetEase(Ease.OutBack));

            currentSequence.OnComplete(() =>
            {
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }
            });

            await currentSequence.AsyncWaitForCompletion();
        }

        public override async UniTask PlayCloseAsync()
        {
            Kill();

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            currentSequence = DOTween.Sequence();
            currentSequence.SetUpdate(true);

            if (canvasGroup != null)
                currentSequence.Join(canvasGroup.DOFade(0f, closeDuration));

            if (contentRoot != null)
                currentSequence.Join(contentRoot.DOScale(startScale, closeDuration).SetEase(Ease.InBack));

            await currentSequence.AsyncWaitForCompletion();
        }
    }
}