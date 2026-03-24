using Data.Scheme.Public;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Utility.LevelEditor
{
    /// <summary>
    /// Part class for procedural generation
    /// </summary>
    public partial class LevelEditor
    {
        [TabGroup("Instruments", "Generation", Icon = SdfIconType.GearFill)]
        [Title("Size")]
        
        [TabGroup("Instruments", "Generation")]
        public int weight;

        [TabGroup("Instruments", "Generation")]
        public int height;
        
        [Title("Shift")]

        [TabGroup("Instruments", "Generation"), Range(0f, 1f)]
        public float shiftX;

        [TabGroup("Instruments", "Generation"), Range(0f, 1f)]
        public float shiftY;
        
        [Title("Fill")]
        
        [TabGroup("Instruments", "Generation")]
        [OnValueChanged(nameof(OnTypeCellChanged))]
        public LevelElementType typeLevelElement;
        
        [TabGroup("Instruments", "Generation")]
        [ValueDropdown(nameof(GetElementsList))]
        public LevelElement styleLevelElement;

        [PropertySpace]
        
        [Button]
        [TabGroup("Instruments", "Generation")]
        public void Generate()
        {
            if (!ValidateGenerate())
                return;

            Clean();

            level = LevelGenerator.Generate(
                level: level,
                elementID: GetElementID(styleLevelElement),
                styleLevelElement: styleLevelElement,
                width: weight,
                height: height,
                shiftX: shiftX,
                shiftY: shiftY);
        }

        [Button]
        [TabGroup("Instruments", "Generation")]
        public void Clean()
        {
            level.Clear();
        }
        
        private void OnTypeCellChanged()
        {
            styleLevelElement = null;
        }

        private bool ValidateGenerate()
        {
            if (styleLevelElement == null)
            {
                EditorUtility.DisplayDialog("Помилка генерації", "Оберіть стиль клітинки.", "OK");
                return false;
            }

            if (weight <= 0 || height <= 0)
            {
                EditorUtility.DisplayDialog("Невірний розмір", "width та height мають бути > 0.", "OK");
                return false;
            }

            return true;
        }
    }
}