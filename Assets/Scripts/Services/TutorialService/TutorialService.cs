using System;
using Data.Model.Private;
using Data.Model.Public;
using Data.Scheme.Private;
using Data.Scheme.Public;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using VContainer;

namespace Services.TutorialService
{
    public class TutorialService : ITutorialService
    {
        private readonly Tutorial currentTutorial = new();

        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly IObjectResolver objectResolver;
        private readonly ILogService logService;

        private TutorialPrivateModel privateModel;
        private TutorialPublicModel publicModel;

        private bool IsRunning => currentTutorial.IsRunning;
        
        public TutorialService(IObjectResolver objectResolver, IPublicModelProvider publicModelProvider, 
            IPrivateModelProvider privateModelProvider, ILogService logService)
        {
            this.logService = logService;
            this.objectResolver = objectResolver;
            this.publicModelProvider = publicModelProvider;
            this.privateModelProvider = privateModelProvider;
        }

        public void Initialize()
        {
            publicModel = publicModelProvider.GetModel<TutorialPublicModel>();
            privateModel = privateModelProvider.GetModel<TutorialPrivateModel>();
        }

        public void StartTutorial(TutorialType tutorialType)
        {
            if (tutorialType is TutorialType.None)
            {
                logService.LogError("Tutorial type None can't be started.", LogCategory.Tutorial);
                return;
            }

            if (publicModel is null || privateModel is null)
            {
                logService.LogError("TutorialService is not initialized. Call Init() after model providers are initialized.", LogCategory.Tutorial);
                return;
            }

            if (IsRunning)
            {
                logService.LogError($"Tutorial already running: {currentTutorial}", LogCategory.Tutorial);
                return;
            }
            
            currentTutorial.PrivateScheme = GetPrivateScheme(tutorialType);

            if (currentTutorial.PrivateScheme is null)
            {
                logService.LogError($"Missing tutorial private scheme: {tutorialType}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            if (currentTutorial.PrivateScheme is { IsComplete: true })
            {
                logService.Log($"Tutorial type: {tutorialType} was completed before.", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            currentTutorial.PublicScheme = GetPublicScheme(tutorialType);
            
            if (currentTutorial.PublicScheme is null)
            {
                logService.LogError($"Missing tutorial public scheme: {tutorialType}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            if (currentTutorial.PublicScheme.Steps is null || currentTutorial.PublicScheme.Steps.Count == 0)
            {
                logService.LogError($"Tutorial has no steps: {tutorialType}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            currentTutorial.StepIndex = 0;
            currentTutorial.Type = tutorialType;

            if (!TryGetCurrentStep(out var currentStep))
            {
                logService.LogError($"Missing tutorial step: {currentTutorial}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            StartCurrentStep(currentStep);
        }

        public void StopTutorial()
        {
            if (!IsRunning)
                return;
            
            UnsubscribeFromCurrentStep();
            
            StopInternal();
        }

        private void StopInternal() => 
            currentTutorial.Clear();

        private TutorialPublicScheme GetPublicScheme(TutorialType type) => 
            publicModel.GetScheme(type.ToString());

        private TutorialPrivateScheme GetPrivateScheme(TutorialType type) => 
            privateModel.GetScheme(type.ToString());

        private void UnsubscribeFromCurrentStep()
        {
            if (TryGetCurrentStep(out var currentStep))
                currentStep.Completed -= OnStepCompleted;
        }

        private void CompleteTutorial()
        {
            if (!IsRunning)
                return;

            currentTutorial.PrivateScheme.CompleteTutorial();
            privateModelProvider.SaveModel<TutorialPrivateModel>();

            logService.Log($"Tutorial {currentTutorial} completed.", LogCategory.Tutorial);

            StopInternal();
        }

        private void OnStepCompleted(TutorialStep completedStep)
        {
            if (!IsRunning)
                return;

            completedStep.Completed -= OnStepCompleted;

            MoveToNextStep();
        }

        private bool TryGetCurrentStep(out TutorialStep step)
        {
            step = null;

            if (!IsRunning || currentTutorial.PublicScheme?.Steps is null)
                return false;

            if (currentTutorial.StepIndex < 0 || currentTutorial.StepIndex >= currentTutorial.PublicScheme.Steps.Count)
                return false;

            step = currentTutorial.PublicScheme.Steps[currentTutorial.StepIndex];
            return step != null;
        }

        private void StartCurrentStep(TutorialStep step)
        {
            if (!IsRunning || step is null)
                return;
            
            try
            {
                objectResolver.Inject(step);
                step.Completed += OnStepCompleted;
                step.StartStep();
            }
            catch (Exception e)
            {
                logService.LogError($"Error while starting tutorial step {currentTutorial.StepIndex}: {e}", LogCategory.Tutorial);
                step.Completed -= OnStepCompleted;
                MoveToNextStep();
            }
        }

        private void MoveToNextStep()
        {
            if (!IsRunning)
                return;

            while (IsRunning)
            {
                currentTutorial.StepIndex++;

                if (currentTutorial.PublicScheme?.Steps is null)
                {
                    logService.LogError($"Tutorial has no steps collection: {currentTutorial.Type}", LogCategory.Tutorial);
                    StopInternal();
                    return;
                }

                if (currentTutorial.PublicScheme.Steps.Count <= currentTutorial.StepIndex)
                {
                    CompleteTutorial();
                    return;
                }
                
                if (!TryGetCurrentStep(out var step))
                {
                    logService.LogError($"Missing tutorial step: {currentTutorial}", LogCategory.Tutorial);
                    continue;
                }
                
                StartCurrentStep(step);
                return;
            }
        }
    }
}
