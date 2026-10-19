using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Entropa.World
{
    public interface ITransitionService
    {
        bool IsTransitioning { get; }

        event Action<LocationDefinition> TransitionStarted;

        event Action<LocationDefinition> TransitionCompleted;

        UniTask TransitionAsync(LocationDefinition target, string pointId, CancellationToken cancellationToken = default);
    }
}
