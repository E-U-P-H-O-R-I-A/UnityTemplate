using Sirenix.OdinInspector;
using UnityEngine;

namespace Utility.LevelEditor
{
    public partial class LevelEditor : MonoBehaviour
    {
        [TabGroup("Instruments", "Generation", SdfIconType.GearFill, TabLayouting = TabLayouting.Shrink)]
        [HideLabel, ShowInInspector, PropertyOrder(-1000)]
        private string GenerationTabOrderAnchor => "Generation";

        [TabGroup("Instruments", "Customization", SdfIconType.PaletteFill, TabLayouting = TabLayouting.Shrink)]
        [HideLabel, ShowInInspector, PropertyOrder(-999)]
        private string CustomizationTabOrderAnchor => "Customization";

        [TabGroup("Instruments", "SaveLoad", SdfIconType.Save, TabLayouting = TabLayouting.Shrink)]
        [HideLabel, ShowInInspector, PropertyOrder(-998)]
        private string SaveLoadTabOrderAnchor => "SaveLoad";
        
        private Level level;

        private void Start()
        {
            CreateRootLevel();
            
            InitializeAssetProvider();
            InitializeInstruments();
        }

        private void CreateRootLevel()
        {
            GameObject gridObject = new("Level");
            gridObject.transform.SetParent(transform);
            level = gridObject.AddComponent<Level>();
        }
    }
}