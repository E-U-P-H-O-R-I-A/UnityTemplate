using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.AssetManagement;
using Data.Model;
using Data.Model.Public;
using Data.Scheme.Public;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utility.LevelEditor
{
    /// <summary>
    /// Part class for loading part levels
    /// </summary>
    public partial class LevelEditor
    {
        private LevelElementsPublicModel elementsPublicModel;
        private LevelMaterialPublicModel materialsPublicModel;

        private async void InitializeAssetProvider()
        {
            var modelKeys = await assetsProvider.GetAssetsListByLabel<IPublicModel>(AssetsLabels.DATA);
            var models = await assetsProvider.LoadAll<IPublicModel>(modelKeys);

            elementsPublicModel = models.OfType<LevelElementsPublicModel>().FirstOrDefault();
            materialsPublicModel = models.OfType<LevelMaterialPublicModel>().FirstOrDefault();
        }

        private IEnumerable<ValueDropdownItem<LevelElement>> GetElementsList() =>
            GetElementsByType(typeLevelElement);

        private string GetElementID(LevelElement element) =>
            elementsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.Prefab == element)?.ID ?? string.Empty;

        private string GetMaterialID(Material material) =>
            materialsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.Material == material)?.ID ?? string.Empty;
        
        private LevelElement GetElementPrefabByID(string elementID)
        {
            if (elementsPublicModel?.Schemes == null || string.IsNullOrEmpty(elementID))
                return null;

            return elementsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.ID == elementID)
                ?.Prefab;
        }

        private Material GetMaterialByID(string materialID)
        {
            if (materialsPublicModel?.Schemes == null || string.IsNullOrEmpty(materialID))
                return null;

            return materialsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.ID == materialID)
                ?.Material;
        }

        private IEnumerable<ValueDropdownItem<LevelElement>> GetElementsByType(LevelElementType type)
        {
            if (elementsPublicModel?.Schemes == null)
                return Enumerable.Empty<ValueDropdownItem<LevelElement>>();

            return elementsPublicModel.Schemes
                .Where(scheme => scheme.LevelElementType == type)
                .Select(scheme => new ValueDropdownItem<LevelElement>(scheme.ID, scheme.Prefab));
        }

        private IEnumerable<ValueDropdownItem<Material>> GetMaterials()
        {
            if (materialsPublicModel?.Schemes == null)
                return Enumerable.Empty<ValueDropdownItem<Material>>();

            return materialsPublicModel.Schemes
                .Where(scheme => scheme != null && scheme.Material != null)
                .Select(scheme => new ValueDropdownItem<Material>(scheme.ID, scheme.Material));
        }
    }
}
