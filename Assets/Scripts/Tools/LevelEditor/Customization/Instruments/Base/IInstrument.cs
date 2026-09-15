using UnityEngine;

namespace Tools.LevelEditor.Base
{
    public interface IInstrument
    {
        void Use(Event currentEvent);
    }
}