using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Services.PrivateContainerProvider
{
    public interface IPrivateContainerProvider
    {
        public UniTask Initialize(CancellationToken ct = default);
        public UniTask SaveAll(CancellationToken ct = default);
        public UniTask SaveContainer<TContainer>(CancellationToken ct = default) where TContainer : IPrivateContainer;
        public TContainer GetContainer<TContainer>() where TContainer : IPrivateContainer;
    }
}
