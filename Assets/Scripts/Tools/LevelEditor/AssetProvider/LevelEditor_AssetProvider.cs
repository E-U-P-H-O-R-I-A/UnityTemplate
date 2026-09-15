#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Data;
using Services.AssetProvider;
using Services.PublicContainerProvider;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.LevelEditor
{
    /// <summary>
    /// Part class for loading part levels
    /// </summary>
    public partial class LevelEditor
    {
        private LevelElementPublicContainer elementsPublicContainer;
        private LevelMaterialPublicContainer materialsPublicContainer;

        private async void LoadAssets()
        {
            var containerKeys = await assetsProvider.GetAssetsListByLabel<IPublicContainer>(AssetsLabels.DATA);
            var containers = await assetsProvider.LoadAll<IPublicContainer>(containerKeys);

            elementsPublicContainer = containers.OfType<LevelElementPublicContainer>().FirstOrDefault();
            materialsPublicContainer = containers.OfType<LevelMaterialPublicContainer>().FirstOrDefault();
        }

        private IEnumerable<ValueDropdownItem<LevelElement>> GetElementsList() =>
            GetElementsByType(typeLevelElement);

        private string GetElementID(LevelElement element) =>
            elementsPublicContainer.Records
                .FirstOrDefault(record => record != null && record.Prefab == element)?.Id ?? string.Empty;

        private string GetMaterialID(Material material) =>
            materialsPublicContainer.Records
                .FirstOrDefault(record => record != null && record.Material == material)?.Id ?? string.Empty;
        
        private LevelElement GetElementPrefabByID(string elementID)
        {
            if (elementsPublicContainer?.Records == null || string.IsNullOrEmpty(elementID))
                return null;

            return elementsPublicContainer.Records
                .FirstOrDefault(record => record != null && record.Id == elementID)
                ?.Prefab;
        }

        private Material GetMaterialByID(string materialID)
        {
            if (materialsPublicContainer?.Records == null || string.IsNullOrEmpty(materialID))
                return null;

            return materialsPublicContainer.Records
                .FirstOrDefault(record => record != null && record.Id == materialID)
                ?.Material;
        }

        private IEnumerable<ValueDropdownItem<LevelElement>> GetElementsByType(LevelElementType type)
        {
            if (elementsPublicContainer?.Records == null)
                return Enumerable.Empty<ValueDropdownItem<LevelElement>>();

            return elementsPublicContainer.Records
                .Where(record => record.LevelElementType == type)
                .Select(record => new ValueDropdownItem<LevelElement>(record.Id, record.Prefab));
        }

        private IEnumerable<ValueDropdownItem<Material>> GetMaterials()
        {
            if (materialsPublicContainer?.Records == null)
                return Enumerable.Empty<ValueDropdownItem<Material>>();

            return materialsPublicContainer.Records
                .Where(record => record != null && record.Material != null)
                .Select(record => new ValueDropdownItem<Material>(record.Id, record.Material));
        }
    }
}
#endif
