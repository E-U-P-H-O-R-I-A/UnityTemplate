using UnityEngine;

namespace Utility.MVC
{
    public abstract class View : MonoBehaviour, IView
    {
        public bool IsVisible => gameObject.activeSelf;

        public virtual void Show() =>
            gameObject.SetActive(true);

        public virtual void Hide() =>
            gameObject.SetActive(false);
    }
}
