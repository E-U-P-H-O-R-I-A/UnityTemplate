using UnityEngine;

namespace Utility.LevelEditor.Base
{
    public interface IInstrument
    {
        void Use(Event currentEvent);
    }
}