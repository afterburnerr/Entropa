using System.Collections.Generic;
using Entropa.Combat;
using Entropa.Core;
using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class CharacterBrain : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _config;
        [SerializeField] private MovesetDefinition _moveset;
        [SerializeField] private MonoBehaviour _intentSourceBehaviour;
        [SerializeField] private MonoBehaviour _animatorBehaviour;

        private CharacterMotor _motor;
        private CharacterStamina _stamina;
        private CharacterTargeting _targeting;
        private Hurtbox _hurtbox;
        private HitboxEmitter _hitboxEmitter;
        private InvulnerabilityController _invulnerability;
        private Health _health;
        private ICharacterAnimator _animator;
        private StateMachine<CharacterBrain> _stateMachine;

        public CharacterConfig Config => _config;

        public MovesetDefinition Moveset => _moveset;

        public CharacterMotor Motor => _motor;

        public CharacterStamina Stamina => _stamina;

        public CharacterTargeting Targeting => _targeting;

        public HitboxEmitter HitboxEmitter => _hitboxEmitter;

        public InvulnerabilityController Invulnerability => _invulnerability;

        public ICharacterAnimator Animator => _animator;

        public IntentBuffer Buffer { get; } = new IntentBuffer();

        public CharacterIntent CurrentIntent { get; private set; }

        public bool IsDead { get; private set; }

        public IdleState Idle { get; private set; }

        public MoveState Move { get; private set; }

        public RollState Roll { get; private set; }

        public AttackState Attack { get; private set; }

        public HitReactState HitReact { get; private set; }

        public DeadState Dead { get; private set; }

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            _stamina = GetComponent<CharacterStamina>();
            _targeting = GetComponent<CharacterTargeting>();
            _hurtbox = GetComponent<Hurtbox>();
            _hitboxEmitter = GetComponent<HitboxEmitter>();
            _invulnerability = GetComponent<InvulnerabilityController>();
            _health = GetComponent<Health>();
            _animator = _animatorBehaviour as ICharacterAnimator;

            if (!ValidateDependencies())
            {
                enabled = false;
                return;
            }

            _stateMachine = new StateMachine<CharacterBrain>(this);
            Idle = new IdleState();
            Move = new MoveState();
            Roll = new RollState();
            Attack = new AttackState();
            HitReact = new HitReactState();
            Dead = new DeadState();

            _hurtbox.Damaged += OnDamaged;
            _health.Died += OnDied;
        }

        private void Start()
        {
            if (_stateMachine != null)
            {
                _stateMachine.ChangeState(Idle);
            }
        }

        private void OnDestroy()
        {
            if (_hurtbox != null)
            {
                _hurtbox.Damaged -= OnDamaged;
            }

            if (_health != null)
            {
                _health.Died -= OnDied;
            }
        }

        private void Update()
        {
            IIntentSource source = ResolveIntentSource();

            if (source == null)
            {
                return;
            }

            CharacterIntent intent = source.GetIntent(Time.deltaTime);
            CurrentIntent = intent;

            if (intent.RollPressed)
            {
                Buffer.NotifyPressed(CharacterAction.Roll, Time.time);
            }

            if (intent.LightAttackPressed)
            {
                Buffer.NotifyPressed(CharacterAction.LightAttack, Time.time);
            }

            if (intent.HeavyAttackPressed)
            {
                Buffer.NotifyPressed(CharacterAction.HeavyAttack, Time.time);
            }

            _stateMachine.Tick(Time.deltaTime);
        }

        public void ChangeState(IState<CharacterBrain> state)
        {
            _stateMachine.ChangeState(state);
        }

        public bool TryStartRoll()
        {
            if (!Buffer.ConsumeIfPressedWithin(CharacterAction.Roll, Time.time, _config.RollInputBuffer))
            {
                return false;
            }

            if (!_stamina.TrySpend(_config.RollStaminaCost))
            {
                return false;
            }

            ChangeState(Roll);
            return true;
        }

        public bool TryStartAttack()
        {
            bool light = Buffer.ConsumeIfPressedWithin(CharacterAction.LightAttack, Time.time, _config.AttackInputBuffer);
            bool heavy = !light && Buffer.ConsumeIfPressedWithin(CharacterAction.HeavyAttack, Time.time, _config.AttackInputBuffer);

            if (!light && !heavy)
            {
                return false;
            }

            IReadOnlyList<AttackDefinition> combo = light ? _moveset.LightCombo : _moveset.HeavyCombo;

            if (combo.Count == 0)
            {
                Debug.LogError($"{name}: moveset '{_moveset.name}' has no {(light ? "light" : "heavy")} attacks.", this);
                return false;
            }

            AttackDefinition attack = combo[0];

            if (!_stamina.TrySpend(attack.StaminaCost))
            {
                return false;
            }

            Attack.Configure(attack);
            ChangeState(Attack);
            return true;
        }

        private void OnDamaged(DamageInfo damage)
        {
            if (IsDead)
            {
                return;
            }

            ChangeState(HitReact);
        }

        private void OnDied()
        {
            IsDead = true;
            Buffer.Clear();
            ChangeState(Dead);
        }

        private IIntentSource ResolveIntentSource()
        {
            return _intentSourceBehaviour as IIntentSource;
        }

        private bool ValidateDependencies()
        {
            bool valid = true;
            valid &= Require(_config, nameof(_config));
            valid &= Require(_moveset, nameof(_moveset));
            valid &= Require(_motor, nameof(CharacterMotor));
            valid &= Require(_stamina, nameof(CharacterStamina));
            valid &= Require(_targeting, nameof(CharacterTargeting));
            valid &= Require(_hurtbox, nameof(Hurtbox));
            valid &= Require(_hitboxEmitter, nameof(HitboxEmitter));
            valid &= Require(_invulnerability, nameof(InvulnerabilityController));
            valid &= Require(_health, nameof(Health));
            valid &= Require(ResolveIntentSource(), nameof(IIntentSource));
            valid &= Require(_animator, nameof(ICharacterAnimator));
            return valid;
        }

        private bool Require<T>(T dependency, string label)
            where T : class
        {
            if (dependency != null)
            {
                return true;
            }

            Debug.LogError($"{name}: dependency '{label}' is missing on {nameof(CharacterBrain)}.", this);
            return false;
        }
    }
}
