using UnityEngine;

namespace Entropa.Character
{
    [CreateAssetMenu(menuName = "Entropa/Character Config", fileName = "CharacterConfig")]
    public sealed class CharacterConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 1.6f;
        [SerializeField] private float _runSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 6.4f;
        [SerializeField] private float _acceleration = 28f;
        [SerializeField] private float _deceleration = 36f;
        [SerializeField] private float _turnSpeedDegrees = 720f;
        [SerializeField] private float _lockOnTurnSpeedDegrees = 1080f;
        [SerializeField] private float _gravity = -22f;

        [Header("Stamina")]
        [SerializeField] private float _staminaMax = 100f;
        [SerializeField] private float _staminaRegenPerSecond = 22f;
        [SerializeField] private float _staminaRegenDelay = 0.6f;
        [SerializeField] private float _sprintDrainPerSecond = 14f;
        [SerializeField] private float _rollStaminaCost = 18f;

        [Header("Roll")]
        [SerializeField] private float _rollDuration = 0.6f;
        [SerializeField] private float _rollSpeed = 7.5f;
        [SerializeField] private AnimationCurve _rollSpeedCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.35f));
        [SerializeField] private float _rollInvulnerabilityStart = 0.12f;
        [SerializeField] private float _rollInvulnerabilityEnd = 0.42f;

        [Header("Input Windows")]
        [SerializeField] private float _rollInputBuffer = 0.35f;
        [SerializeField] private float _attackInputBuffer = 0.35f;

        [Header("Reactions")]
        [SerializeField] private float _hitReactDuration = 0.35f;

        [Header("Targeting")]
        [SerializeField] private float _lockOnRadius = 14f;
        [SerializeField] private float _lockOnReleaseRadius = 18f;
        [SerializeField] private float _lockOnConeHalfAngle = 70f;
        [SerializeField] private float _targetSwitchCooldown = 0.25f;

        public float WalkSpeed => _walkSpeed;

        public float RunSpeed => _runSpeed;

        public float SprintSpeed => _sprintSpeed;

        public float Acceleration => _acceleration;

        public float Deceleration => _deceleration;

        public float TurnSpeedDegrees => _turnSpeedDegrees;

        public float LockOnTurnSpeedDegrees => _lockOnTurnSpeedDegrees;

        public float Gravity => _gravity;

        public float StaminaMax => _staminaMax;

        public float StaminaRegenPerSecond => _staminaRegenPerSecond;

        public float StaminaRegenDelay => _staminaRegenDelay;

        public float SprintDrainPerSecond => _sprintDrainPerSecond;

        public float RollStaminaCost => _rollStaminaCost;

        public float RollDuration => _rollDuration;

        public float RollSpeed => _rollSpeed;

        public AnimationCurve RollSpeedCurve => _rollSpeedCurve;

        public float RollInvulnerabilityStart => _rollInvulnerabilityStart;

        public float RollInvulnerabilityEnd => _rollInvulnerabilityEnd;

        public float RollInputBuffer => _rollInputBuffer;

        public float AttackInputBuffer => _attackInputBuffer;

        public float HitReactDuration => _hitReactDuration;

        public float LockOnRadius => _lockOnRadius;

        public float LockOnReleaseRadius => _lockOnReleaseRadius;

        public float LockOnConeHalfAngle => _lockOnConeHalfAngle;

        public float TargetSwitchCooldown => _targetSwitchCooldown;
    }
}
