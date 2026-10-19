using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entropa.Core;
using UnityEngine;
using VContainer;

namespace Entropa.World
{
    public sealed class TransitionPreloader : MonoBehaviour
    {
        [SerializeField] private LocationDefinition _target;
        [SerializeField] private Collider _zone;

        private ISceneLoader _sceneLoader;
        private CancellationTokenSource _preloadCts;
        private bool _overlapChecked;

        public event Action<LocationDefinition> Ready;

        public LocationDefinition Target => _target;

        [Inject]
        public void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            if (_zone != null)
            {
                _zone.isTrigger = true;
            }
        }

        private void OnDestroy()
        {
            _preloadCts?.Cancel();
            _preloadCts?.Dispose();
            _preloadCts = null;
        }

        private void Update()
        {
            if (_overlapChecked)
            {
                return;
            }

            _overlapChecked = true;
            if (_sceneLoader != null && _target != null && IsPlayerInsideZone())
            {
                HandleTrigger();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_sceneLoader == null || _target == null || other.gameObject.layer != GameLayers.Player)
            {
                return;
            }

            HandleTrigger();
        }

        private void HandleTrigger()
        {
            if (_sceneLoader.IsActivated(_target))
            {
                Ready?.Invoke(_target);
                return;
            }

            Preload();
        }

        private bool IsPlayerInsideZone()
        {
            if (_zone == null)
            {
                return false;
            }

            Bounds bounds = _zone.bounds;
            Collider[] hits = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                _zone.transform.rotation,
                GameLayers.PlayerMask,
                QueryTriggerInteraction.Collide);
            return hits.Length > 0;
        }

        private void Preload()
        {
            if (_preloadCts != null)
            {
                return;
            }

            _preloadCts = new CancellationTokenSource();
            RunPreloadAsync(_preloadCts.Token).Forget();
        }

        private async UniTaskVoid RunPreloadAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _sceneLoader.PreloadAsync(_target, cancellationToken);
                Ready?.Invoke(_target);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
