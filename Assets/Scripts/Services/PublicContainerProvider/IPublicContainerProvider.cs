using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Services.PublicContainerProvider
{
    public interface IPublicContainerProvider : IService
    {
        public UniTask Initialize(CancellationToken ct = default);
        public TContainer GetContainer<TContainer>() where TContainer : IPublicContainer;
    }
}