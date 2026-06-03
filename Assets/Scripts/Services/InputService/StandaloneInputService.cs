using UnityEngine;
using UnityEngine.InputSystem;

namespace Services.InputService
{
    internal sealed class StandaloneInputService : InputService
    {
        private Mouse mouse;

        public override void Initialize()
        {
            mouse = Mouse.current;
        }

        public override void Tick()
        {
            base.Tick();
            
            if (State == States.Disabled)
                return;
            
            HandleMousePointer();
            HandleScrollZoom();
        }

        private void HandleMousePointer()
        {
            Vector2 mousePosition = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame) 
                BeginPointer(mousePosition);

            if (State is States.Idle or States.Disabled)
                return;

            UpdatePointer(mousePosition);

            if (mouse.leftButton.wasReleasedThisFrame) 
                EndPointer(mousePosition);
        }

        private void HandleScrollZoom()
        {
            float wheelDelta = mouse.scroll.ReadValue().y;

            if (Mathf.Abs(wheelDelta) <= Mathf.Epsilon)
                return;

            PublishZoom(mouse.position.ReadValue(), wheelDelta);
        }
    }
}
