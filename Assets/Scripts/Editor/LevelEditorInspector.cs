using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Utility.LevelEditor;

namespace Editor
{
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorInspector : OdinEditor
    {
        private void OnSceneGUI()
        {
            LevelEditor levelEditor = (LevelEditor)target;
            Event currentEvent = Event.current;
    
            levelEditor.HandleSceneInput(currentEvent);
        }
    }
}