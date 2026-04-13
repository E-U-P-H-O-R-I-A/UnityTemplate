using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.WindowsService.Animation
{
    public abstract class BaseWindowAnimation : MonoBehaviour
    {
        public abstract void Kill();
        public abstract UniTask PlayOpenAsync();
        public abstract UniTask PlayCloseAsync();
    }
}