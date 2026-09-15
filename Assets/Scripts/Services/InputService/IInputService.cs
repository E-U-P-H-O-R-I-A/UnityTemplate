using System;

namespace Services.InputService
{
    public interface IInputService : IInitializableService
    {
        event Action<InputPointerEventData> Clicked;
        event Action<InputSwipeEventData> Swiped;
        event Action<InputZoomEventData> Zoomed;
        
        event Action<InputDragEventData> DragStarted;
        event Action<InputDragEventData> DragEnded;
        event Action<InputDragEventData> Dragged;
        
        void SetInputStatus(bool isEnabled);
    }
}
