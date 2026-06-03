using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemTouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Services.InputService
{
    internal sealed class MobileInputService : InputService
    {
        private float? previousPinchDistance;

        public override void Tick()
        {
            base.Tick();

            if (State == States.Disabled)
                return;
            
            TouchFrame touches = ReadTouchFrame();

            HandlePinchZoom(touches);
            HandleSingleTouchPointer(touches);
        }

        protected override void OnInputDisabled()
        {
            previousPinchDistance = null;
        }

        private void HandleSingleTouchPointer(TouchFrame touches)
        {
            if (touches.Count != 1)
            {
                CancelPointer();
                return;
            }

            TouchFrameData touch = touches.PrimaryTouch;

            if (State == States.Idle && !IsEndedOrCanceled(touch.Phase))
            {
                BeginPointer(touch.Position);
            }

            if (State == States.Idle)
            {
                return;
            }

            UpdatePointer(touch.Position);

            if (IsEndedOrCanceled(touch.Phase))
            {
                EndPointer(touch.Position);
            }
        }

        private void HandlePinchZoom(TouchFrame touches)
        {
            if (!touches.HasPinch)
            {
                previousPinchDistance = null;
                return;
            }

            Vector2 center = (touches.PrimaryTouch.Position + touches.SecondaryTouch.Position) * 0.5f;
            float pinchDistance = Vector2.Distance(touches.PrimaryTouch.Position, touches.SecondaryTouch.Position);

            if (!previousPinchDistance.HasValue)
            {
                previousPinchDistance = pinchDistance;
                return;
            }

            float delta = pinchDistance - previousPinchDistance.Value;
            previousPinchDistance = pinchDistance;

            if (Mathf.Abs(delta) > Mathf.Epsilon)
            {
                PublishZoom(center, delta);
            }
        }

        private TouchFrame ReadTouchFrame()
        {
            TouchFrame touchFrame = default;

            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                return touchFrame;
            }

            var touches = touchscreen.touches;

            for (int i = 0; i < touches.Count; i++)
            {
                var touchControl = touches[i];
                InputSystemTouchPhase phase = touchControl.phase.ReadValue();
                bool isPressed = touchControl.press.isPressed;

                if (!isPressed && !IsRelevantTouchPhase(phase))
                {
                    continue;
                }

                touchFrame.Add(new TouchFrameData(
                    touchControl.touchId.ReadValue(),
                    touchControl.position.ReadValue(),
                    phase));

                if (touchFrame.HasPinch)
                {
                    break;
                }
            }

            return touchFrame;
        }

        private static bool IsRelevantTouchPhase(InputSystemTouchPhase phase)
        {
            return phase is InputSystemTouchPhase.Began
                or InputSystemTouchPhase.Moved
                or InputSystemTouchPhase.Stationary
                or InputSystemTouchPhase.Ended
                or InputSystemTouchPhase.Canceled;
        }

        private static bool IsEndedOrCanceled(InputSystemTouchPhase phase) => 
            phase is InputSystemTouchPhase.Ended or InputSystemTouchPhase.Canceled;

        private readonly struct TouchFrameData
        {
            public int TouchId { get; }
            public Vector2 Position { get; }
            public InputSystemTouchPhase Phase { get; }

            public TouchFrameData(int touchId, Vector2 position, InputSystemTouchPhase phase)
            {
                TouchId = touchId;
                Position = position;
                Phase = phase;
            }
        }

        private struct TouchFrame
        {
            public int Count { get; private set; }
            public TouchFrameData PrimaryTouch { get; private set; }
            public TouchFrameData SecondaryTouch { get; private set; }
            public bool HasPinch => Count > 1;

            public void Add(TouchFrameData touch)
            {
                if (Count == 0)
                {
                    PrimaryTouch = touch;
                }
                else if (Count == 1)
                {
                    SecondaryTouch = touch;
                }

                Count++;
            }
        }
    }
}
