using Cysharp.Threading.Tasks;

namespace Utility.StateMachine
{
    public interface IPayloadState<in TPayload> : IExitableState
    {
        UniTask Enter(TPayload payload);
    }
}
