using HapticId = Data.HapticPublicContainer.Id;

namespace Services.HapticService
{
    public interface IHapticService
    {
        bool IsEnabled { get;}

        void Initialize();
        void ToggleHaptic();
        void PlayPreset(HapticId preset);
        void PlayCustom(HapticSetting setting);
        void PlaySequence(HapticSequence sequence);
    }
}
