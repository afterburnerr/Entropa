using System;
using UnityEngine;

namespace Entropa.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class Hurtbox : MonoBehaviour, ITargetable
    {
        [SerializeField] private float _targetPointHeight = 1.2f;

        private Health _health;
        private InvulnerabilityController _invulnerability;

        public event Action<DamageInfo> Damaged;

        public bool IsTargetable => _health != null && _health.IsAlive;

        public Transform TargetTransform => transform;

        public Vector3 TargetPoint => transform.position + Vector3.up * _targetPointHeight;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _invulnerability = GetComponent<InvulnerabilityController>();
        }

        private void OnEnable()
        {
            TargetRegistry.Register(this);
        }

        private void OnDisable()
        {
            TargetRegistry.Unregister(this);
        }

        public bool TryReceiveDamage(in DamageInfo damage)
        {
            if (_health == null || !_health.CanReceiveDamage)
            {
                return false;
            }

            if (_invulnerability != null && _invulnerability.IsInvulnerable)
            {
                return false;
            }

            _health.ReceiveDamage(in damage);
            Damaged?.Invoke(damage);
            return true;
        }
    }
}
