using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Utility.LoadingCurtain
{
    public class LoadingCurtainView : View
    {
        private static readonly int MaskSizeId = Shader.PropertyToID("_MaskSize");

        [Space, Header("Refs")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CanvasGroup contentGroup;
        [SerializeField] private Graphic curtainGraphic;
        [SerializeField] private Slider progressFill;

        [Space, Header("Settings")]
        [SerializeField] private float closedMaskSize;
        [SerializeField] private float openedMaskSize = 1f;
        [SerializeField] private float closeDuration = 0.6f;
        [SerializeField] private float openDuration = 0.8f;
        [SerializeField] private float contentFadeDuration = 0.25f;
        [SerializeField] private Ease maskEase = Ease.InOutSine;

        private Material runtimeMaterial;
        private Tween currentTween;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (CreateRuntimeMaterial())
                SetClosed();
        }

        private void OnDestroy()
        {
            Kill();

            if (runtimeMaterial != null)
                Destroy(runtimeMaterial);
        }

        public override void Show()
        {
            base.Show();

            canvasGroup.alpha = 1f;
        }

        public override void Hide()
        {
            canvasGroup.alpha = 0f;

            base.Hide();
        }

        public void SetProgress(float value) =>
            progressFill.value = value;

        public void SetContentAlpha(float value) =>
            contentGroup.alpha = value;

        public void SetClosed() =>
            SetMaskSize(closedMaskSize);

        public void SetOpened() =>
            SetMaskSize(openedMaskSize);

        public void Kill()
        {
            if (currentTween == null || !currentTween.IsActive())
                return;

            currentTween.Kill();
            currentTween = null;
        }

        public UniTask PlayCloseAsync() =>
            PlayMaskAsync(closedMaskSize, closeDuration);

        public UniTask PlayOpenAsync() =>
            PlayMaskAsync(openedMaskSize, openDuration);

        public async UniTask FadeContentAsync(float target)
        {
            if (contentFadeDuration <= 0f)
            {
                SetContentAlpha(target);
                return;
            }

            await contentGroup
                .DOFade(target, contentFadeDuration)
                .SetUpdate(true)
                .AsyncWaitForCompletion();
        }

        private async UniTask PlayMaskAsync(float target, float duration)
        {
            Kill();

            if (!CreateRuntimeMaterial() || duration <= 0f)
            {
                SetMaskSize(target);
                return;
            }

            currentTween = DOTween
                .To(GetMaskSize, SetMaskSize, target, duration)
                .SetEase(maskEase)
                .SetUpdate(true);

            await currentTween.AsyncWaitForCompletion();
        }

        private bool CreateRuntimeMaterial()
        {
            if (runtimeMaterial != null)
                return true;

            Material source = curtainGraphic != null ? curtainGraphic.material : null;

            if (source == null || !source.HasProperty(MaskSizeId))
            {
                Debug.LogError($"{nameof(LoadingCurtainView)} requires a UI material with the _MaskSize property.", this);
                return false;
            }

            runtimeMaterial = new Material(source)
            {
                name = $"{source.name} (Runtime)"
            };

            curtainGraphic.material = runtimeMaterial;

            return true;
        }

        private float GetMaskSize() =>
            runtimeMaterial != null ? runtimeMaterial.GetFloat(MaskSizeId) : closedMaskSize;

        private void SetMaskSize(float value)
        {
            if (runtimeMaterial != null)
                runtimeMaterial.SetFloat(MaskSizeId, value);
        }
    }
}
