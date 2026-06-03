using UnityEngine;

namespace Services.InputService
{
    public readonly struct InputDragEventData
    {
        public Vector2 StartPosition { get; }
        public Vector2 Position { get; }
        public Vector2 Delta { get; }
        public float Duration { get; }

        public InputDragEventData(Vector2 startPosition, Vector2 position, Vector2 delta, float duration)
        {
            StartPosition = startPosition;
            Duration = duration;
            Position = position;
            Delta = delta;
        }
    }
}