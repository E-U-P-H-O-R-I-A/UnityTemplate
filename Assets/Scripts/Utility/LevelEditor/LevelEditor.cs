using Services.AssetProvider;
using Services.LogService;
using Services.PublicContainerProvider;
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

        private ILogService logService;
        private IAssetsProvider assetsProvider;

        private Level level;

        private void Start()
        {
            InitServices();
            LoadAssets();
            CreateRootLevel();
            InitializeInstruments();
        }
        
        private void InitServices()
        {
            logService = new LogService();
            assetsProvider = new AssetsProvider(logService);
        }

        private void CreateRootLevel()
        {
            GameObject gridObject = new("Level");
            gridObject.transform.SetParent(transform);
            level = gridObject.AddComponent<Level>();
        }
    }
}