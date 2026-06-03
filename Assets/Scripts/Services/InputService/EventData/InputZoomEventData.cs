using UnityEngine;

namespace Services.InputService
{
    public readonly struct InputZoomEventData
    {
        public Vector2 Position { get; }
        public float Delta { get; }

        public InputZoomEventData(Vector2 position, float delta)
        {
            Position = position;
            Delta = delta;
        }
    }
}