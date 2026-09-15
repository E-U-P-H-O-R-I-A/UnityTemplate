using Services.LogService;
using Utility.StateMachine;

namespace Infrastructure
{
    public class GameStateMachine : StateMachine
    {
        public GameStateMachine(ILogService logService) : base(logService) { }
    }
}
