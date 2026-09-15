using HapticId = Data.HapticPublicContainer.Id;

namespace Services.HapticService
{
    public interface IHapticService : IInitializableService
    {
        bool IsEnabled { get;}

        void ToggleHaptic();
        void PlayPreset(HapticId preset);
        void PlayCustom(HapticSetting setting);
        void PlaySequence(HapticSequence sequence);
    }
}
