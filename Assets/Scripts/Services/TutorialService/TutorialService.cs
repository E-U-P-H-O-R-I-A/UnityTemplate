using System;
using Data;
using Services.LogService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
using VContainer;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Services.TutorialService
{
    public class TutorialService : ITutorialService
    {
        private readonly Tutorial currentTutorial = new();

        private readonly IPrivateContainerProvider privateContainerProvider;
        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly IObjectResolver objectResolver;
        private readonly ILogService logService;

        private TutorialPrivateContainer privateContainer;
        private TutorialPublicContainer publicContainer;

        private bool IsRunning => currentTutorial.IsRunning;
        
        public TutorialService(IObjectResolver objectResolver, IPublicContainerProvider publicContainerProvider, 
            IPrivateContainerProvider privateContainerProvider, ILogService logService)
        {
            this.logService = logService;
            this.objectResolver = objectResolver;
            this.publicContainerProvider = publicContainerProvider;
            this.privateContainerProvider = privateContainerProvider;
        }

        public void Initialize()
        {
            publicContainer = publicContainerProvider.GetContainer<TutorialPublicContainer>();
            privateContainer = privateContainerProvider.GetContainer<TutorialPrivateContainer>();
        }

        public void StartTutorial(TutorialId tutorialType)
        {
            if (tutorialType is TutorialId.None)
            {
                logService.LogError("Tutorial type None can't be started.", LogCategory.Tutorial);
                return;
            }

            if (publicContainer is null || privateContainer is null)
            {
                logService.LogError("TutorialService is not initialized. Call Init() after container providers are initialized.", LogCategory.Tutorial);
                return;
            }

            if (IsRunning)
            {
                logService.LogError($"Tutorial already running: {currentTutorial}", LogCategory.Tutorial);
                return;
            }
            
            currentTutorial.PrivateRecord = GetPrivateRecord(tutorialType);

            if (currentTutorial.PrivateRecord is null)
            {
                logService.LogError($"Missing tutorial private record: {tutorialType}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            if (currentTutorial.PrivateRecord is { IsComplete: true })
            {
                logService.Log($"Tutorial type: {tutorialType} was completed before.", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            currentTutorial.PublicRecord = GetPublicRecord(tutorialType);
            
            if (currentTutorial.PublicRecord is null)
            {
                logService.LogError($"Missing tutorial public record: {tutorialType}", LogCategory.Tutorial);
                StopInternal();
                return;
            }

            if (currentTutorial.PublicRecord.Steps is null || currentTutorial.PublicRecord.Steps.Count == 0)
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

        private TutorialPublicRecord GetPublicRecord(TutorialId type) => 
            publicContainer.GetRecord(type);

        private TutorialPrivateRecord GetPrivateRecord(TutorialId type) => 
            privateContainer.GetRecord(type.ToString());

        private void UnsubscribeFromCurrentStep()
        {
            if (TryGetCurrentStep(out var currentStep))
                currentStep.Completed -= OnStepCompleted;
        }

        private void CompleteTutorial()
        {
            if (!IsRunning)
                return;

            currentTutorial.PrivateRecord.Complete();
            privateContainerProvider.SaveContainer<TutorialPrivateContainer>();

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

            if (!IsRunning || currentTutorial.PublicRecord?.Steps is null)
                return false;

            if (currentTutorial.StepIndex < 0 || currentTutorial.StepIndex >= currentTutorial.PublicRecord.Steps.Count)
                return false;

            step = currentTutorial.PublicRecord.Steps[currentTutorial.StepIndex];
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

                if (currentTutorial.PublicRecord?.Steps is null)
                {
                    logService.LogError($"Tutorial has no steps collection: {currentTutorial.Type}", LogCategory.Tutorial);
                    StopInternal();
                    return;
                }

                if (currentTutorial.PublicRecord.Steps.Count <= currentTutorial.StepIndex)
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
