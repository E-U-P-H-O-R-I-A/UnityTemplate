using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Utility.GridEditor
{
    public class GridEditor : MonoBehaviour
    {
        private const string CellsRoot = "Assets/Resources/Cells";
        
        private Grid grid;
        
        #region Generation

        [TabGroup("Instruments", "Generation", SdfIconType.GearFill, TabLayouting = TabLayouting.Shrink)]
        
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
        [ValueDropdown("GetCellTypes"), OnValueChanged(nameof(OnTypeCellChanged))]
        public string typeCell;
        
        [TabGroup("Instruments", "Generation"), ValueDropdown("GetStylePrefabs")]
        public Cell styleCell;
        
        [OnInspectorGUI, TabGroup("Instruments", "Generation")] 
        private void Space1() { GUILayout.Space(10); }
        
        [Button(ButtonSizes.Large), ButtonGroup("Instruments/Generation/Buttons")]
        public void Generate()
        {
            if (styleCell == null)
            {
                EditorUtility.DisplayDialog(
                    "Помилка генерації",           
                    "Будь ласка, оберіть стиль клітинки перед генерацією.",
                    "ОК"                           
                );
                return;
            }
            
            if (weight <= 0 || height <= 0)
            {
                EditorUtility.DisplayDialog(
                    "Невірний розмір сітки",
                    "Поля 'weight' та 'height' повинні бути більше 0.",
                    "Зрозуміло"
                );
                return;
            }

            Clean();
            
            GameObject gridObject = new("Grid");
            gridObject.transform.SetParent(transform);
            grid = gridObject.AddComponent<Grid>();

            Vector3 cellSize = styleCell.transform.localScale;

            float halfW = (weight - 1) * cellSize.x * 0.5f;
            float halfH = (height - 1) * cellSize.z * 0.5f;
            
            List<Cell> cells = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < weight; x++)
                {
                    Cell cell = Instantiate(styleCell, grid.transform);
                    
                    float shiftX = y % 2 == 0 ? this.shiftX * cellSize.x : 0;
                    float shiftY = x % 2 == 0 ? this.shiftY * cellSize.z : 0;
                    Vector3 localPos = new(x * cellSize.x + shiftX - halfW, 0f, y * cellSize.z + shiftY - halfH);

                    cell.transform.localPosition = localPos;
                    cell.name = $"Cell {x} {y}";
                    cells.Add(cell);
                }
            }

            grid.Initialize(cells);
        }
        
        [OnInspectorGUI, ButtonGroup("Instruments/Generation/Buttons")] 
        private void Space2() { GUILayout.Space(10); }
        
        [Button(ButtonSizes.Large), ButtonGroup("Instruments/Generation/Buttons")]
        public void Clean()
        {
            if (grid == null) 
                return;
            
            Destroy(grid.gameObject);
        }
        
        #endregion

        #region Customisation
        //[TabGroup("Instruments", "Customisation", SdfIconType.PaletteFill, TabLayouting = TabLayouting.Shrink, )]
        
        //public void 

        #endregion

        #region Other

        private void OnTypeCellChanged() => styleCell = null;
        
        private IEnumerable<ValueDropdownItem<string>> GetCellTypes()
        {
            if (!AssetDatabase.IsValidFolder(CellsRoot))
                yield break;
            
            string[] folderGuids = AssetDatabase.FindAssets("t:folder", new[] { CellsRoot });

            foreach (var guid in folderGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == CellsRoot) continue;
                
                string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
                if (parent == CellsRoot)
                {
                    string folderName = Path.GetFileName(path);
                    yield return new ValueDropdownItem<string>($"{folderName}", path);
                }
            }
        }
        
        private IEnumerable<ValueDropdownItem<Cell>> GetStylePrefabs()
        {
            if (string.IsNullOrEmpty(typeCell) || !AssetDatabase.IsValidFolder(typeCell))
                yield break;
            
            string[] prefabGuids = AssetDatabase.FindAssets("t:prefab", new[] { typeCell });

            foreach (var guid in prefabGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var parent = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");
                if (parent != typeCell) continue;

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab == null) continue;
                
                var cell = prefab.GetComponent<Cell>();
                if (cell == null) continue;

                string styleName = Path.GetFileNameWithoutExtension(assetPath);
                yield return new ValueDropdownItem<Cell>(styleName, cell);
            }
        }

        #endregion
    }
}