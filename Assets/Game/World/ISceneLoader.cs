using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Entropa.World
{
    public interface ISceneLoader
    {
        UniTask PreloadAsync(LocationDefinition location, CancellationToken cancellationToken = default);

        UniTask ActivateAsync(LocationDefinition location, CancellationToken cancellationToken = default);

        UniTask UnloadAsync(LocationDefinition location, CancellationToken cancellationToken = default);

        bool IsActivated(LocationDefinition location);

        bool TryGetLoaded(LocationDefinition location, out Scene scene);
    }
}
