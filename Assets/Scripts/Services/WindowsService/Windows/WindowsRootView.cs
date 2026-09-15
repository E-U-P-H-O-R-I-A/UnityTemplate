using UnityEngine;
using Utility.MVC;

namespace Services.WindowsService.Windows
{
    public class WindowsRootView : View
    {
        [SerializeField] private Canvas canvas;

        public Transform Container => canvas.transform;

        private void Awake() =>
            DontDestroyOnLoad(gameObject);
    }
}
