using System;
using UnityEditor;
using UnityEngine;
using Utility.LevelEditor.Base;

namespace Utility.LevelEditor
{
    [Serializable]
    public class Eraser : IInstrument
    {
        private Level level;

        public void Initialize(Level level) =>
            this.level = level;

        public void Use(Event currentEvent)
        {
            if (level == null)
                return;

            if (currentEvent == null || currentEvent.button != 0)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                return;

            LevelElement element = hit.collider.GetComponentInParent<LevelElement>();

            if (element == null)
                return;

            level.RemoveElement(element);
            Undo.DestroyObjectImmediate(element.gameObject);

            currentEvent.Use();
        }
    }
}