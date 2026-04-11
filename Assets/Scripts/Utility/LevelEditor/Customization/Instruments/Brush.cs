using System;
using System.Collections.Generic;
using Data.Scheme.Public;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Utility.LevelEditor.Base;
using Object = UnityEngine.Object;

namespace Utility.LevelEditor
{
    [Serializable]
    public class Brush : IInstrument, IEnterable, IExitable, IRotatable, IUpdatable
    {
        [Space]
        [OnValueChanged(nameof(UpdateStyleList))]
        [SerializeField] private LevelElementType typeLevelElement;

        [ValueDropdown(nameof(GetAvailableStyles))]
        [OnValueChanged(nameof(UpdatePreviewItem))]
        [SerializeField] private LevelElement styleLevelElement;

        [ValueDropdown(nameof(GetAvailableMaterials))]
        [OnValueChanged(nameof(UpdatePreviewItem))]
        [LabelText("Material")]
        [SerializeField] private Material materialLevelElement;

        private Level level;
        private LevelElement previewItem;

        private Func<Material, string> getMaterialID;
        private Func<LevelElement, string> getElementID;
        private Func<IEnumerable<ValueDropdownItem<Material>>> getMaterials;
        private Func<LevelElementType, IEnumerable<ValueDropdownItem<LevelElement>>> getStyles;

        public void Initialize(
            Level level,
            Func<Material, string> getMaterialID,
            Func<LevelElement, string> getElementID,
            Func<IEnumerable<ValueDropdownItem<Material>>> getMaterials,
            Func<LevelElementType, IEnumerable<ValueDropdownItem<LevelElement>>> getStyles)
        {
            this.getElementID = getElementID;
            this.level = level;
            this.getStyles = getStyles;
            this.getMaterials = getMaterials;
            this.getMaterialID = getMaterialID;
        }

        public void Enter() =>
            CreatePreview();

        public void Exit() =>
            DestroyPreview();

        public void Rotate() =>
            RotatePreview();

        public void Use(Event currentEvent) =>
            Place();

        public void Update(Event currentEvent) =>
            UpdatePreviewPosition(currentEvent);

        private void RotatePreview() =>
            previewItem?.transform.Rotate(0f, 90f, 0f);

        private IEnumerable<ValueDropdownItem<Material>> GetAvailableMaterials() =>
            getMaterials?.Invoke() ?? Array.Empty<ValueDropdownItem<Material>>();

        private IEnumerable<ValueDropdownItem<LevelElement>> GetAvailableStyles() =>
            getStyles?.Invoke(typeLevelElement) ?? Array.Empty<ValueDropdownItem<LevelElement>>();

        private void UpdateStyleList()
        {
            styleLevelElement = null;
            DestroyPreview();
        }

        private void UpdatePreviewItem()
        {
            DestroyPreview();
            CreatePreview();
        }

        private void CreatePreview()
        {
            if (styleLevelElement == null || previewItem != null)
                return;

            previewItem = Object.Instantiate(styleLevelElement);
            previewItem.name = $"{styleLevelElement.name}_Preview";
            previewItem.SetStatusCollider(false);

            if (materialLevelElement != null)
            {
                var materialID = getMaterialID?.Invoke(materialLevelElement) ?? string.Empty;
                previewItem.SetMaterial(materialID, materialLevelElement);
            }
        }

        private void DestroyPreview()
        {
            if (previewItem == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(previewItem.gameObject);
            else
                Object.DestroyImmediate(previewItem.gameObject);

            previewItem = null;
        }

        private void UpdatePreviewPosition(Event currentEvent)
        {
            if (previewItem == null)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                return;

            LevelElement targetElement = hit.collider.GetComponentInParent<LevelElement>();

            if (targetElement != null)
            {
                Vector3 position = GetSnappedPosition(targetElement, hit.point);
                position.y = previewItem.transform.position.y;
                previewItem.transform.position = position;
            }
            else
            {
                previewItem.transform.position = hit.point;
            }

            SceneView.RepaintAll();
        }

        private Vector3 GetSnappedPosition(LevelElement targetElement, Vector3 hitPoint)
        {
            Renderer targetRenderer = targetElement.Renderer;
            Renderer previewRenderer = previewItem.Renderer;

            if (targetRenderer == null || previewRenderer == null)
                return hitPoint;

            Bounds targetBounds = targetRenderer.bounds;
            Bounds previewBounds = previewRenderer.bounds;

            Vector3 center = targetBounds.center;
            Vector3 directionToHit = hitPoint - center;

            float absX = Mathf.Abs(directionToHit.x);
            float absZ = Mathf.Abs(directionToHit.z);

            Vector3 snappedPosition = center;

            if (absX > absZ)
            {
                float signX = Mathf.Sign(directionToHit.x);
                float offsetX = targetBounds.extents.x + previewBounds.extents.x;
                snappedPosition += Vector3.right * signX * offsetX;
            }
            else
            {
                float signZ = Mathf.Sign(directionToHit.z);
                float offsetZ = targetBounds.extents.z + previewBounds.extents.z;
                snappedPosition += Vector3.forward * signZ * offsetZ;
            }

            snappedPosition.y = targetBounds.center.y;
            return snappedPosition;
        }

        private void Place()
        {
            if (styleLevelElement == null || previewItem == null)
                return;

            LevelElement instance = Object.Instantiate(
                styleLevelElement,
                previewItem.transform.position,
                previewItem.transform.rotation);

            instance.name = styleLevelElement.name;
            instance.SetElementID(getElementID.Invoke(styleLevelElement));
            
            if (materialLevelElement != null)
            {
                var materialID = getMaterialID?.Invoke(materialLevelElement) ?? string.Empty;
                instance.SetMaterial(materialID, materialLevelElement);
            }

            level.AddElement(instance);
            Undo.RegisterCreatedObjectUndo(instance.gameObject, "Place Level Element");
        }
    }
}
