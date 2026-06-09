using System;
using UnityEngine;

namespace Services.InputService
{
    public abstract class InputService : IInputService
    {
        protected enum States
        {
            Idle,
            Pressed,
            Dragging,
            Disabled
        }
        
        private const float DRAG_START_THRESHOLD = 8f;
        private const float SWIPE_MIN_DISTANCE = 80f;
        private const float SWIPE_MAX_DURATION = 0.3f;
        
        public event Action<InputPointerEventData> Clicked;
        public event Action<InputSwipeEventData> Swiped;
        public event Action<InputZoomEventData> Zoomed;
        
        public event Action<InputDragEventData> DragStarted;
        public event Action<InputDragEventData> DragEnded;
        public event Action<InputDragEventData> Dragged;

        protected States State { get; set; } = States.Disabled;

        private float duration;
        private float pointerDownTime;
        
        private Vector2 totalDelta;
        private Vector2 pointerPosition;
        private Vector2 pointerDownPosition;

        public virtual void Tick() { }

        protected virtual void OnInputDisabled() { }

        public virtual void Initialize()
        {
            State = States.Idle; 
        }

        public void SetInputStatus(bool isEnabled)
        {
            bool isCurrentlyEnabled = State != States.Disabled;

            if (isCurrentlyEnabled == isEnabled)
                return;

            if (!isEnabled)
            {
                CancelPointer();
                State = States.Disabled;
                OnInputDisabled();
            }
            else
            {
                State = States.Idle;
            }
        }

        protected void PublishZoom(Vector2 position, float delta) => 
            Zoomed?.Invoke(new InputZoomEventData(position, delta));

        protected void PublishClick() => 
            Clicked?.Invoke(new InputPointerEventData(pointerPosition));
        
        protected void PublicDragging() => 
            Dragged?.Invoke(new InputDragEventData(pointerDownPosition, pointerPosition, totalDelta, duration));

        protected void PublishDragEnded(Vector2 totalDelta, float duration) => 
            DragEnded?.Invoke(new InputDragEventData(pointerDownPosition, pointerPosition, totalDelta, duration));

        protected void PublishDragStarted(Vector2 totalDelta, float duration) => 
            DragStarted?.Invoke(new InputDragEventData(pointerDownPosition, pointerPosition, totalDelta, duration));

        protected void PublishSwipe(Vector2 delta) => 
            Swiped?.Invoke(new InputSwipeEventData(pointerDownPosition, pointerPosition, delta, GetSwipeDirection(delta)));
        
        protected void CancelPointer()
        {
            if (State is States.Idle or States.Disabled)
                return;

            if (State == States.Dragging)
            {
                SaveData();
                PublishDragEnded(totalDelta, duration);
            }
            
            ClearPointerState();
        }
        
        protected void BeginPointer(Vector2 position)
        {
            if (State == States.Disabled)
                return;

            State = States.Pressed;
            SaveStartData(position);
        }
        
        protected void UpdatePointer(Vector2 position)
        {
            if (State is States.Idle or States.Disabled)
                return;
            
            SaveData(position);
            
            if (State == States.Dragging)
            {
                PublicDragging();
            }
            else if (HasDragConditions(totalDelta, duration))
            {
                State = States.Dragging;
                PublishDragStarted(totalDelta, duration);
            }
            
        }
        
        protected void EndPointer(Vector2 position)
        {
            if (State is States.Idle or States.Disabled)
                return;

            SaveData(position);
            
            if (State == States.Dragging)
            {
                PublishDragEnded(totalDelta, duration);
            }
            else if (HasSwipeConditions(totalDelta, duration))
            {
                PublishSwipe(totalDelta);
            }
            else if (HasDragConditions(totalDelta, duration))
            {
                PublishDragStarted(totalDelta, duration);
                PublishDragEnded(totalDelta, duration);
            }
            else
            {
                PublishClick();
            }

            ClearPointerState();
        }

        private void ClearPointerState() => 
            State = States.Idle;

        private bool HasDragConditions(Vector2 delta, float duration) => 
            delta.sqrMagnitude >= DRAG_START_THRESHOLD * DRAG_START_THRESHOLD && duration > SWIPE_MAX_DURATION;

        private bool HasSwipeConditions(Vector2 delta, float duration) =>
            delta.sqrMagnitude >= SWIPE_MIN_DISTANCE * SWIPE_MIN_DISTANCE && duration <= SWIPE_MAX_DURATION;

        private void SaveStartData(Vector3 position)
        {
            pointerDownPosition = position;
            pointerPosition = position;
            pointerDownTime = Time.unscaledTime;
        }

        private void SaveData(Vector2 position = default)
        {
            if (position != default)
                pointerPosition = position;
            
            duration = Time.unscaledTime - pointerDownTime;
            totalDelta = pointerPosition - pointerDownPosition;
        }

        private static InputSwipeDirection GetSwipeDirection(Vector2 delta)
        {
            float horizontalDistance = Mathf.Abs(delta.x);
            float verticalDistance = Mathf.Abs(delta.y);
            
            if (horizontalDistance >= verticalDistance)
            {
                return delta.x > 0f ? InputSwipeDirection.Left : InputSwipeDirection.Right;
            }

            return delta.y > 0f ? InputSwipeDirection.Down : InputSwipeDirection.Up;
        }
    }
}
