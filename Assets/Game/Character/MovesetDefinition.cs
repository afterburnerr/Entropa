using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entropa.Character
{
    [Serializable]
    public sealed class AttackDefinition
    {
        [SerializeField] private string _name = "Light Attack";
        [SerializeField] private float _damage = 20f;
        [SerializeField] private float _poiseDamage = 20f;
        [SerializeField] private float _staminaCost = 15f;
        [SerializeField] private float _startupDuration = 0.25f;
        [SerializeField] private float _activeDuration = 0.15f;
        [SerializeField] private float _recoveryDuration = 0.35f;
        [SerializeField] private float _cancelStartTime = 0.5f;
        [SerializeField] private float _moveSpeedMultiplier = 0.25f;
        [SerializeField] private float _hitRadius = 0.7f;
        [SerializeField] private Vector3 _hitOffset = new Vector3(0f, 1f, 0.7f);

        public string Name => _name;

        public float Damage => _damage;

        public float PoiseDamage => _poiseDamage;

        public float StaminaCost => _staminaCost;

        public float MoveSpeedMultiplier => _moveSpeedMultiplier;

        public float HitRadius => _hitRadius;

        public Vector3 HitOffset => _hitOffset;

        public ActionTiming Timing => new ActionTiming(_startupDuration, _activeDuration, _recoveryDuration, _cancelStartTime);
    }

    [CreateAssetMenu(menuName = "Entropa/Moveset", fileName = "Moveset")]
    public sealed class MovesetDefinition : ScriptableObject
    {
        [SerializeField] private List<AttackDefinition> _lightCombo = new List<AttackDefinition>();
        [SerializeField] private List<AttackDefinition> _heavyCombo = new List<AttackDefinition>();

        public IReadOnlyList<AttackDefinition> LightCombo => _lightCombo;

        public IReadOnlyList<AttackDefinition> HeavyCombo => _heavyCombo;
    }
}
