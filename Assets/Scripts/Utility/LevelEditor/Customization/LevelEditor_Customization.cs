using Sirenix.OdinInspector;
using UnityEngine;
using Utility.LevelEditor.Base;

namespace Utility.LevelEditor
{
    /// <summary>
    /// Part class for customization level
    /// </summary>
    public partial class LevelEditor
    {
        private IInstrument currentInstrument;
        private bool ignoreNextMouseDown;
        private bool wasSceneViewFocused;
        
        [TabGroup("Instruments", "Customization", Icon = SdfIconType.PaletteFill)]
        [PropertySpace]
        
        [EnumToggleButtons, HideLabel]
        [TabGroup("Instruments", "Customization")]
        [OnValueChanged(nameof(OnInstrumentChanged))]
        [SerializeField] private InstrumentType instrumentType;
        
        [ShowIf("@instrumentType == InstrumentType.Brush")]
        [TabGroup("Instruments", "Customization"), HideLabel] 
        [SerializeField] private Brush brush;

        [ShowIf("@instrumentType == InstrumentType.Eraser")]
        [TabGroup("Instruments", "Customization"), HideLabel] 
        [SerializeField] private Eraser eraser;

        [ShowIf("@instrumentType == InstrumentType.Palette")]
        [TabGroup("Instruments", "Customization"), HideLabel] 
        [SerializeField] private Palette palette;
        
        public void HandleSceneInput(Event currentEvent)
        {
            currentInstrument ??= GetInstrument(instrumentType);

            bool isSceneViewFocused = UnityEditor.EditorWindow.focusedWindow is UnityEditor.SceneView;

            if (isSceneViewFocused && !wasSceneViewFocused)
                ignoreNextMouseDown = true;

            wasSceneViewFocused = isSceneViewFocused;
            
            (currentInstrument as IUpdatable)?.Update(currentEvent);
            
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                if (ignoreNextMouseDown)
                {
                    ignoreNextMouseDown = false;
                    currentEvent.Use();
                    return;
                }
                
                currentInstrument?.Use(currentEvent);
                currentEvent.Use();
            }
            
            if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.R)
            {
                (currentInstrument as IRotatable)?.Rotate();
                currentEvent.Use();
            }
        }
        
        private void InitializeInstruments()
        {
            eraser.Initialize(level);
            palette.Initialize(GetMaterialID, GetMaterials);
            brush.Initialize(level, GetMaterialID, GetElementID, GetMaterials, GetElementsByType);
        }
        
        private void OnInstrumentChanged()
        {
            ignoreNextMouseDown = true;
            (currentInstrument as IExitable)?.Exit();
            currentInstrument = GetInstrument(instrumentType);
            (currentInstrument as IEnterable)?.Enter();
        }
        
        private IInstrument GetInstrument(InstrumentType type)
        {
            return type switch
            {
                InstrumentType.Brush => brush,
                InstrumentType.Eraser => eraser,
                InstrumentType.Palette => palette,
                _ => brush
            };
        }
    }
}