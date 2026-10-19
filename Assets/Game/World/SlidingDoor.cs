using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Entropa.World
{
    public sealed class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Transform _panel;
        [SerializeField] private TransitionPreloader _preloader;
        [SerializeField] private Vector3 _openOffset = new Vector3(0f, 2.4f, 0f);
        [SerializeField] private float _duration = 0.5f;

        private Vector3 _closedLocalPosition;
        private CancellationTokenSource _animationCts;

        private void Awake()
        {
            if (_panel != null)
            {
                _closedLocalPosition = _panel.localPosition;
            }
        }

        private void OnEnable()
        {
            if (_preloader != null)
            {
                _preloader.Ready += HandleReady;
            }
        }

        private void OnDisable()
        {
            if (_preloader != null)
            {
                _preloader.Ready -= HandleReady;
            }

            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = null;
        }

        private void HandleReady(LocationDefinition location)
        {
            if (_panel == null)
            {
                return;
            }

            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            AnimateAsync(_closedLocalPosition + _openOffset, _animationCts.Token).Forget();
        }

        private async UniTaskVoid AnimateAsync(Vector3 target, CancellationToken cancellationToken)
        {
            Vector3 start = _panel.localPosition;
            float elapsed = 0f;

            try
            {
                while (elapsed < _duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / _duration);
                    _panel.localPosition = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, progress));
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                _panel.localPosition = target;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
