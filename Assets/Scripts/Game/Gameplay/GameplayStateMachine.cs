using Services.LogService;
using Utility.StateMachine;

namespace Game.Gameplay
{
    public class GameplayStateMachine : StateMachine
    {
        public GameplayStateMachine(ILogService logService) : base(logService) { }
    }
}
