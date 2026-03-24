using Sirenix.OdinInspector;

namespace Utility.LevelEditor.Base
{
    public enum InstrumentType
    {
        [LabelText(SdfIconType.BrushFill)] Brush = 0,
        [LabelText(SdfIconType.Palette2)] Palette = 1,
        [LabelText(SdfIconType.EraserFill)] Eraser = 2,
    }
}