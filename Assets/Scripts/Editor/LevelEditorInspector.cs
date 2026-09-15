using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Tools.LevelEditor;

namespace EditorTools
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