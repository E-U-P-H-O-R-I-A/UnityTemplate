using UnityEngine;

namespace Services.InputService
{
    public enum InputSwipeDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
    
    public readonly struct InputSwipeEventData
    {
        public InputSwipeDirection Direction { get; }
        public Vector2 StartPosition { get; }
        public Vector2 Position { get; }
        public Vector2 Delta { get; }

        public InputSwipeEventData(Vector2 startPosition, Vector2 position, Vector2 delta, InputSwipeDirection direction)
        {
            StartPosition = startPosition;
            Direction = direction;
            Position = position;
            Delta = delta;
        }
    }
}
