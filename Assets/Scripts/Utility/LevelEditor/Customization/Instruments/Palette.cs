using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Utility.LevelEditor.Base;

namespace Utility.LevelEditor
{
    [Serializable]
    public class Palette : IInstrument
    {
        [ValueDropdown(nameof(GetAvailableMaterials))]
        [LabelText("Material"), SerializeField] private Material materialLevelElement;

        private Func<Material, int> getMaterialID;
        private Func<IEnumerable<ValueDropdownItem<Material>>> getMaterials;

        public void Initialize(
            Func<Material, int> getMaterialID,
            Func<IEnumerable<ValueDropdownItem<Material>>> getMaterials)
        {
            this.getMaterialID = getMaterialID;
            this.getMaterials = getMaterials;
        }

        public void Use(Event currentEvent)
        {
            if (materialLevelElement == null)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                return;

            LevelElement targetElement = hit.collider.GetComponentInParent<LevelElement>();
            if (targetElement == null)
                return;

            int materialID = getMaterialID?.Invoke(materialLevelElement) ?? 0;

            Undo.RecordObject(targetElement, "Paint Level Element");
            targetElement.SetMaterial(materialID, materialLevelElement);
            EditorUtility.SetDirty(targetElement);
        }

        private IEnumerable<ValueDropdownItem<Material>> GetAvailableMaterials() =>
            getMaterials?.Invoke() ?? Array.Empty<ValueDropdownItem<Material>>();
    }
}