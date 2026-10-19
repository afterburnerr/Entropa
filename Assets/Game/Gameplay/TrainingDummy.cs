using Entropa.Combat;
using UnityEngine;

namespace Entropa.Gameplay
{
    [RequireComponent(typeof(Health), typeof(Hurtbox))]
    public sealed class TrainingDummy : MonoBehaviour
    {
        [SerializeField] private Transform _visual;

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= OnDied;
            }
        }

        private void OnDied()
        {
            if (_visual == null)
            {
                return;
            }

            _visual.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }
}
