using System;
using VContainer.Unity;

namespace Services.InputService
{
    public interface IInputService : IService, ITickable
    {
        event Action<InputPointerEventData> Clicked;
        event Action<InputSwipeEventData> Swiped;
        event Action<InputZoomEventData> Zoomed;
        
        event Action<InputDragEventData> DragStarted;
        event Action<InputDragEventData> DragEnded;
        event Action<InputDragEventData> Dragged;
        
        void Initialize();
        
        void SetInputStatus(bool isEnabled);
    }
}
