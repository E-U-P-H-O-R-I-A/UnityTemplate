using UnityEngine;

namespace Utility.LevelEditor.Base
{
    public interface IUpdatable
    {
        void Update(Event currentEvent);
    }
}