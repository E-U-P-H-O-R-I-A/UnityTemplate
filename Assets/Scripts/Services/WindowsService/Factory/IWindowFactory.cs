using UnityEngine;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService.Factory
{
    public interface IWindowFactory
    {
        Window Create(WindowType type, Transform parent);
    }
}
