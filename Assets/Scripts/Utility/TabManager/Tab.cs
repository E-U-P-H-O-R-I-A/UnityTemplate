using UnityEngine;

namespace Utility.TabManager
{
    public abstract class Tab : MonoBehaviour
    {
        public abstract TabType Type { get; }

        public virtual void Initialize() { }

        public virtual void Show() => 
            gameObject.SetActive(true);

        public virtual void Hide() => 
            gameObject.SetActive(false);
    }
}