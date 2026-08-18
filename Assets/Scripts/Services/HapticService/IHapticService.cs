using HapticType = Data.HapticPublicModel.Type;

namespace Services.HapticService
{
    public interface IHapticService
    {
        bool IsEnabled { get;}

        void Initialize();
        void ToggleHaptic();
        void PlayPreset(HapticType preset);
        void PlayCustom(HapticSetting setting);
        void PlaySequence(HapticSequence sequence);
    }
}
