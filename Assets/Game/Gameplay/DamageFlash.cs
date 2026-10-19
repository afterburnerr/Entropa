using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entropa.Combat;
using UnityEngine;

namespace Entropa.Gameplay
{
    [RequireComponent(typeof(Hurtbox))]
    public sealed class DamageFlash : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.1f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private Hurtbox _hurtbox;
        private MaterialPropertyBlock _block;
        private CancellationTokenSource _flashCancellation;

        private void Awake()
        {
            _hurtbox = GetComponent<Hurtbox>();
            _hurtbox.Damaged += OnDamaged;
            _block = new MaterialPropertyBlock();
        }

        private void OnDestroy()
        {
            if (_hurtbox != null)
            {
                _hurtbox.Damaged -= OnDamaged;
            }

            CancelFlash();
        }

        private void OnDamaged(DamageInfo damage)
        {
            if (_renderer == null)
            {
                return;
            }

            CancelFlash();
            _flashCancellation = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            FlashAsync(_flashCancellation.Token).Forget();
        }

        private async UniTaskVoid FlashAsync(CancellationToken token)
        {
            SetFlash(true);

            bool canceled = await UniTask.Delay(
                TimeSpan.FromSeconds(_flashDuration),
                cancellationToken: token).SuppressCancellationThrow();

            if (canceled)
            {
                return;
            }

            SetFlash(false);
        }

        private void CancelFlash()
        {
            if (_flashCancellation == null)
            {
                return;
            }

            _flashCancellation.Cancel();
            _flashCancellation.Dispose();
            _flashCancellation = null;
        }

        private void SetFlash(bool on)
        {
            if (on)
            {
                _block.SetColor(BaseColorId, _flashColor);
                _renderer.SetPropertyBlock(_block);
                return;
            }

            _block.Clear();
            _renderer.SetPropertyBlock(_block);
        }
    }
}
