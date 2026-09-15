using UnityEngine;

namespace Tools.LevelEditor.Base
{
    public interface IUpdatable
    {
        void Update(Event currentEvent);
    }
}