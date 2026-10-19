using System;
using UnityEngine;

namespace Entropa.Combat
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;

        public float Current { get; private set; }

        public float Max => _maxHealth;

        public bool IsAlive => Current > 0f;

        public bool CanReceiveDamage => IsAlive;

        public event Action<DamageInfo> Damaged;

        public event Action Died;

        private void Awake()
        {
            Current = _maxHealth;
        }

        public void ReceiveDamage(in DamageInfo damage)
        {
            if (!CanReceiveDamage)
            {
                return;
            }

            float amount = Mathf.Max(0f, damage.Amount);
            if (amount <= 0f)
            {
                return;
            }

            Current = Mathf.Max(0f, Current - amount);
            Damaged?.Invoke(damage);

            if (!IsAlive)
            {
                Died?.Invoke();
            }
        }
    }
}
