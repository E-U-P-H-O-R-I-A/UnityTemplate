using UnityEngine;

namespace Services.InputService
{
    public readonly struct InputPointerEventData
    {
        public Vector2 Position { get; }

        public InputPointerEventData(Vector2 position)
        {
            Position = position;
        }
    }
}