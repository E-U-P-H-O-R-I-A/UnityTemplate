using Cysharp.Threading.Tasks;
using Data.Model.Private;
using Data.Scheme.Private;
using Data.Scheme.Public;
using Infrastructure;
using Lofelt.NiceVibrations;
using Services.PrivateModelProvider;
using UnityEngine;

namespace Services.HapticService
{
    public class HapticService : IHapticService
    {
        private const float MIN_INTERVAL = 0.05f;

        private readonly IPrivateModelProvider privateModelProvider;
        
        private float lastPlayTime;

        public bool IsEnabled => 
            HapticController.hapticsEnabled;

        public HapticService(IPrivateModelProvider privateModelProvider) => 
            this.privateModelProvider = privateModelProvider;

        public void Initialize() => 
            HapticController.Init();

        public void ToggleHaptic()
        {
            bool value = !HapticController.hapticsEnabled;
            
            HapticController.hapticsEnabled = value;
            
            GetScheme().Haptic = value;
            privateModelProvider.SaveModel<SettingPrivateModel>();
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

        public void PlayPreset(HapticType preset)
        {
            if (!IsCanPlay())
                return;
            
            switch (preset)
            {
                case HapticType.Selection:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
                    break;
                case HapticType.Success:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
                    break;
                case HapticType.Warning:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
                    break;
                case HapticType.Failure:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
                    break;
                case HapticType.LightImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                    break;
                case HapticType.MediumImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                    break;
                case HapticType.HeavyImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                    break;
                case HapticType.RigidImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
                    break;
                case HapticType.SoftImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                    break;
            }
        }
        
        private SettingPrivateScheme GetScheme() => 
            privateModelProvider.GetModel<SettingPrivateModel>().GetScheme();
        
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
