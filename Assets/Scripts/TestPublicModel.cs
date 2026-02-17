using Data.Model;
using Data.Model.Public;
using Data.Scheme.Public;
using Services.Provider.Public;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DefaultNamespace
{
    public class TestPublicModel : MonoBehaviour
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private Image spriteRenderer;

        private IPublicModelProvider publicModelProvider;

        [Inject]
        public void Construct(IPublicModelProvider publicModelProvider)
        {
            this.publicModelProvider = publicModelProvider;
        }

        [Button]
        public void UpdateIcon()
        {
            if (publicModelProvider.GetModel(out CurrencyPublicModel model))
            {
                if (model.GetScheme(type.ToString(), out CurrencyPublicScheme scheme))
                {
                    spriteRenderer.sprite = scheme.Sprite;
                }
            }
        }
    }
}