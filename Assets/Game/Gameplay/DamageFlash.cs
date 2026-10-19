using System.Collections;
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
        private Coroutine _flashRoutine;

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
        }

        private void OnDamaged(DamageInfo damage)
        {
            if (_renderer == null)
            {
                return;
            }

            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _flashRoutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            SetFlash(true);
            yield return new WaitForSeconds(_flashDuration);
            SetFlash(false);
            _flashRoutine = null;
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
