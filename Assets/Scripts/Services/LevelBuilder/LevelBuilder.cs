using System.Linq;
using Data;
using Services.LogService;
using Services.PublicContainerProvider;
using Tools.LevelEditor;
using UnityEngine;
using Utility.Factory;

namespace Services.LevelBuilder
{
    public class LevelBuilder : ILevelBuilder
    {
        private const string LEVEL_ROOT_NAME = "Level";

        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly ILogService logService;
        private readonly IFactory factory;

        private LevelElementPublicContainer elementsContainer;
        private LevelMaterialPublicContainer materialsContainer;

        private Level currentLevel;

        public LevelBuilder(IPublicContainerProvider publicContainerProvider, IFactory factory, ILogService logService)
        {
            this.publicContainerProvider = publicContainerProvider;
            this.logService = logService;
            this.factory = factory;
        }

        public Level CurrentLevel => currentLevel;

        public Level Build(LevelPublicRecord record, Transform parent = null)
        {
            Clear();

            if (record == null)
            {
                logService.LogError("[LevelBuilder] Level record is null", LogCategory.Gameplay);
                return null;
            }

            LevelSaveData saveData = record.GetSaveData();

            if (saveData?.Elements == null)
            {
                logService.LogError($"[LevelBuilder] Level {record.Id} has no valid json data", LogCategory.Gameplay);
                return null;
            }

            elementsContainer = publicContainerProvider.GetContainer<LevelElementPublicContainer>();
            materialsContainer = publicContainerProvider.GetContainer<LevelMaterialPublicContainer>();

            currentLevel = CreateLevelRoot(parent);

            foreach (LevelElementSaveData elementData in saveData.Elements)
            {
                LevelElement element = CreateElement(elementData);

                if (element == null)
                    continue;

                currentLevel.AddElement(element);
            }

            logService.Log($"[LevelBuilder] Level {record.Id} built with {saveData.Elements.Count} elements", LogCategory.Gameplay);

            return currentLevel;
        }

        public void Clear()
        {
            if (currentLevel == null)
                return;

            currentLevel.Clear();

            Object.Destroy(currentLevel.gameObject);

            currentLevel = null;
        }

        private Level CreateLevelRoot(Transform parent)
        {
            GameObject rootObject = new(LEVEL_ROOT_NAME);

            if (parent != null)
                rootObject.transform.SetParent(parent, false);

            return rootObject.AddComponent<Level>();
        }

        private LevelElement CreateElement(LevelElementSaveData data)
        {
            LevelElement prefab = GetPrefab(data.ElementID);

            if (prefab == null)
            {
                logService.LogWarning($"[LevelBuilder] Element prefab not found by Id: {data.ElementID}", LogCategory.Gameplay);
                return null;
            }

            LevelElement instance = factory.CreateFromPrefab(prefab);

            Transform cachedTransform = instance.transform;
            cachedTransform.position = data.Position;
            cachedTransform.eulerAngles = data.Rotation;
            cachedTransform.localScale = data.Scale;

            ApplyMaterial(instance, data.MaterialID);

            return instance;
        }

        private void ApplyMaterial(LevelElement element, string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return;

            Material material = materialsContainer.Records
                .FirstOrDefault(record => record != null && record.Id == materialID)
                ?.Material;

            if (material == null)
            {
                logService.LogWarning($"[LevelBuilder] Material not found by Id: {materialID}", LogCategory.Gameplay);
                return;
            }

            element.SetMaterial(materialID, material);
        }

        private LevelElement GetPrefab(string elementID)
        {
            if (string.IsNullOrEmpty(elementID))
                return null;

            return elementsContainer.Records
                .FirstOrDefault(record => record != null && record.Id == elementID)
                ?.Prefab;
        }
    }
}
