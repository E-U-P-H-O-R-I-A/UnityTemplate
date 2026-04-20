using System.IO;
using Services.LogService;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Utility.LevelEditor
{
    /// <summary>
    /// Part class for save / load level json
    /// </summary>
    public partial class LevelEditor
    {
        [TabGroup("Instruments", "SaveLoad", Icon = SdfIconType.Save)]
        
        [PropertySpace]
        
        [TabGroup("Instruments", "SaveLoad")]
        [Button("Save Level To Json", ButtonSizes.Large)]
        private void SaveLevelToJson()
        {
            if (level == null)
            {
                logService.LogError("Level is null", LogCategory.LevelEditor);
                return;
            }

            string path = EditorUtility.SaveFilePanel(
                "Save level to json",
                Application.dataPath,
                "LevelData",
                "json");

            if (string.IsNullOrEmpty(path))
                return;

            LevelSaveData saveData = BuildSaveData();
            string json = JsonUtility.ToJson(saveData, true);

            File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            logService.Log($"Level saved to: {path}", LogCategory.LevelEditor);
        }

        [TabGroup("Instruments", "SaveLoad")]
        [Button("Load Level From Json", ButtonSizes.Large)]
        private void LoadLevelFromJson()
        {
            if (level == null)
            {
                logService.LogError("Level is null", LogCategory.LevelEditor);
                return;
            }

            string path = EditorUtility.OpenFilePanel(
                "Load level from json",
                Application.dataPath,
                "json");

            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path))
            {
                logService.LogError($"File not found: {path}", LogCategory.LevelEditor);
                return;
            }

            string json = File.ReadAllText(path);
            LevelSaveData saveData = JsonUtility.FromJson<LevelSaveData>(json);

            if (saveData == null || saveData.Elements == null)
            {
                logService.LogError("Failed to parse level json", LogCategory.LevelEditor);
                return;
            }

            RestoreLevel(saveData);

            logService.Log($"Level loaded from: {path}", LogCategory.LevelEditor);
        }

        private LevelSaveData BuildSaveData()
        {
            LevelSaveData saveData = new();

            foreach (LevelElement element in level.GetElements())
            {
                if (element == null)
                    continue;

                Transform cachedTransform = element.transform;

                saveData.Elements.Add(new LevelElementSaveData
                {
                    ElementID = element.ElementID,
                    MaterialID = element.MaterialID,
                    Position = cachedTransform.position,
                    Rotation = cachedTransform.eulerAngles,
                    Scale = cachedTransform.localScale
                });
            }

            return saveData;
        }

        private void RestoreLevel(LevelSaveData saveData)
        {
            level.Clear();

            foreach (LevelElementSaveData elementData in saveData.Elements)
            {
                LevelElement prefab = GetElementPrefabByID(elementData.ElementID);
                if (prefab == null)
                {
                    logService.LogWarning($"Element prefab not found by ID: {elementData.ElementID}", LogCategory.LevelEditor);
                    continue;
                }

                LevelElement instance = InstantiateLevelElement(prefab, elementData);
                if (instance == null)
                    continue;

                ApplyMaterial(instance, elementData.MaterialID);
                level.AddElement(instance);
            }
        }

        private LevelElement InstantiateLevelElement(LevelElement prefab, LevelElementSaveData data)
        {
            LevelElement instance;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                instance = (LevelElement)PrefabUtility.InstantiatePrefab(prefab);
            else
                instance = Object.Instantiate(prefab);
#else
            instance = Instantiate(prefab);
#endif

            if (instance == null)
                return null;

            Transform cachedTransform = instance.transform;
            cachedTransform.position = data.Position;
            cachedTransform.eulerAngles = data.Rotation;
            cachedTransform.localScale = data.Scale;

            return instance;
        }

        private void ApplyMaterial(LevelElement element, string materialID)
        {
            if (element == null || string.IsNullOrEmpty(materialID))
                return;

            Material material = GetMaterialByID(materialID);
            if (material == null)
            {
                logService.LogWarning($"Material not found by ID: {materialID}", LogCategory.LevelEditor);
                return;
            }

            element.SetMaterial(materialID, material);
        }
    }
}
