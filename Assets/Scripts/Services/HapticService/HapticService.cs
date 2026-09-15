using Cysharp.Threading.Tasks;
using Data;
using Lofelt.NiceVibrations;
using Services.PrivateContainerProvider;
using UnityEngine;
using HapticId = Data.HapticPublicContainer.Id;

namespace Services.HapticService
{
    public class HapticService : IHapticService
    {
        private const float MIN_INTERVAL = 0.05f;

        private readonly IPrivateContainerProvider privateContainerProvider;
        
        private float lastPlayTime;

        public bool IsEnabled => 
            HapticController.hapticsEnabled;

        public HapticService(IPrivateContainerProvider privateContainerProvider) => 
            this.privateContainerProvider = privateContainerProvider;

        public void Initialize() => 
            HapticController.Init();

        public void ToggleHaptic()
        {
            bool value = !HapticController.hapticsEnabled;
            
            HapticController.hapticsEnabled = value;
            
            GetRecord().HapticEnabled = value;
            privateContainerProvider.SaveContainer<SettingsPrivateContainer>();
        }

        public void PlayCustom(HapticSetting setting)
        {
            if (!IsCanPlay())
                return;
            
            HapticPatterns.PlayConstant(setting.Amplitude, setting.Frequency, setting.Duration);
        }

        public async void PlaySequence(HapticSequence sequence)
        {
            if (!IsCanPlay())
                return;
            
            foreach (var setting in sequence.Sequence)
            {
                PlayCustom(setting);
                await UniTask.Delay((int)(setting.Duration * 1000));
            }
        }

        public void PlayPreset(HapticId preset)
        {
            if (!IsCanPlay())
                return;
            
            switch (preset)
            {
                case HapticId.Selection:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
                    break;
                case HapticId.Success:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
                    break;
                case HapticId.Warning:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
                    break;
                case HapticId.Failure:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
                    break;
                case HapticId.LightImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                    break;
                case HapticId.MediumImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                    break;
                case HapticId.HeavyImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                    break;
                case HapticId.RigidImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
                    break;
                case HapticId.SoftImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                    break;
            }
        }
        
        private SettingsPrivateRecord GetRecord() => 
            privateContainerProvider.GetContainer<SettingsPrivateContainer>().GetRecord();
        
        private bool IsCanPlay()
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (Time.unscaledTime - lastPlayTime < MIN_INTERVAL)
            {
                return false;
            }

            lastPlayTime = Time.unscaledTime;
            return true;
        }
    }
}
