using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Entropa.World
{
    public sealed class TransitionService : ITransitionService
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldState _worldState;
        private readonly ITransitionPointLocator _pointLocator;
        private readonly IPlayerAnchor _playerAnchor;

        public TransitionService(
            ISceneLoader sceneLoader,
            IWorldState worldState,
            ITransitionPointLocator pointLocator,
            IPlayerAnchor playerAnchor)
        {
            _sceneLoader = sceneLoader;
            _worldState = worldState;
            _pointLocator = pointLocator;
            _playerAnchor = playerAnchor;
        }

        public bool IsTransitioning { get; private set; }

        public event Action<LocationDefinition> TransitionStarted;

        public event Action<LocationDefinition> TransitionCompleted;

        public async UniTask TransitionAsync(
            LocationDefinition target,
            string pointId,
            CancellationToken cancellationToken = default)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (IsTransitioning)
            {
                return;
            }

            IsTransitioning = true;
            LocationDefinition previous = _worldState.CurrentLocation;

            try
            {
                await _sceneLoader.PreloadAsync(target, cancellationToken);
                await _sceneLoader.ActivateAsync(target, cancellationToken);

                TransitionStarted?.Invoke(target);

                TransitionPoint point = _pointLocator.Find(target.SceneName, pointId);
                if (point == null)
                {
                    Debug.LogWarning($"Transition point '{pointId}' was not found in scene '{target.SceneName}'.");
                }
                else
                {
                    _playerAnchor.Teleport(point.transform.position, point.transform.rotation);
                }

                _worldState.SetLocation(target);

                if (previous != null && previous != target)
                {
                    await _sceneLoader.UnloadAsync(previous, cancellationToken);
                }

                TransitionCompleted?.Invoke(target);
            }
            finally
            {
                IsTransitioning = false;
            }
        }
    }
}
