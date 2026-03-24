using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.AssetManagement;
using Data.Model.Public;
using Data.Scheme.Public;
using Services.AssetProvider;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utility.LevelEditor
{
    /// <summary>
    /// Part class for loading part levels
    /// </summary>
    public partial class LevelEditor
    {
        private IAssetsProvider assetsProvider;
        private LevelElementsPublicModel elementsPublicModel;
        private LevelMaterialPublicModel materialsPublicModel;

        private async void InitializeAssetProvider()
        {
            assetsProvider = new AssetsProvider();

            elementsPublicModel = await assetsProvider.Load<LevelElementsPublicModel>(AssetsLabels.LEVEL_EDITOR_ELEMENTS);
            materialsPublicModel = await assetsProvider.Load<LevelMaterialPublicModel>(AssetsLabels.LEVEL_EDITOR_MATERIALS);
        }

        private IEnumerable<ValueDropdownItem<LevelElement>> GetElementsList() =>
            GetElementsByType(typeLevelElement);

        private int GetElementID(LevelElement element) =>
            elementsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.Prefab == element).ID;

        private int GetMaterialID(Material material) =>
            materialsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.Material == material).ID;
        
        private LevelElement GetElementPrefabByID(int elementID)
        {
            if (elementsPublicModel?.Schemes == null)
                return null;

            return elementsPublicModel.Schemes
                .FirstOrDefault(scheme => scheme != null && scheme.ID == elementID)
                ?.Prefab;
        }

        private Material GetMaterialByID(int materialID)
        {
            if (materialsPublicModel?.Schemes == null)
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
                .Select(scheme => new ValueDropdownItem<LevelElement>(scheme.StringID, scheme.Prefab));
        }

        private IEnumerable<ValueDropdownItem<Material>> GetMaterials()
        {
            if (materialsPublicModel?.Schemes == null)
                return Enumerable.Empty<ValueDropdownItem<Material>>();

            return materialsPublicModel.Schemes
                .Where(scheme => scheme != null && scheme.Material != null)
                .Select(scheme => new ValueDropdownItem<Material>(scheme.StringID, scheme.Material));
        }
    }
}