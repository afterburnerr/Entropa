using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class AttackState : IState<CharacterBrain>
    {
        private AttackDefinition _attack;
        private ActionTimeline _timeline;
        private bool _hitWindowOpen;

        public void Configure(AttackDefinition attack)
        {
            _attack = attack;
        }

        public void Enter(CharacterBrain context)
        {
            _timeline = new ActionTimeline(_attack.Timing);
            _hitWindowOpen = false;
            context.Animator.PlayAttack(_attack);
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            _timeline.Advance(deltaTime);

            UpdateHitWindow(context);
            UpdateSteering(context, deltaTime);

            if (_timeline.IsCancelWindowOpen && context.TryStartRoll())
            {
                return;
            }

            if (_timeline.IsFinished)
            {
                if (context.CurrentIntent.MoveDirection.sqrMagnitude > 0.0001f)
                {
                    context.ChangeState(context.Move);
                }
                else
                {
                    context.ChangeState(context.Idle);
                }
            }
        }

        public void Exit(CharacterBrain context)
        {
            if (_hitWindowOpen)
            {
                _hitWindowOpen = false;
                context.HitboxEmitter.EndWindow();
            }
        }

        private void UpdateHitWindow(CharacterBrain context)
        {
            bool active = _timeline.IsActive;

            if (active && !_hitWindowOpen)
            {
                _hitWindowOpen = true;
                context.HitboxEmitter.BeginWindow(_attack.Damage, _attack.PoiseDamage, _attack.HitOffset, _attack.HitRadius);
            }
            else if (!active && _hitWindowOpen)
            {
                _hitWindowOpen = false;
                context.HitboxEmitter.EndWindow();
            }
        }

        private void UpdateSteering(CharacterBrain context, float deltaTime)
        {
            CharacterConfig config = context.Config;
            Vector3 moveDirection = context.CurrentIntent.MoveDirection;

            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                context.Motor.SetTargetPlanarVelocity(moveDirection * _attack.MoveSpeedMultiplier * config.RunSpeed);
                context.Motor.FaceDirection(moveDirection, config.TurnSpeedDegrees, deltaTime);
                return;
            }

            context.Motor.SetTargetPlanarVelocity(Vector3.zero);

            if (context.Targeting.HasTarget)
            {
                Vector3 toTarget = context.Targeting.TargetPoint - context.transform.position;
                context.Motor.FaceDirection(toTarget, config.LockOnTurnSpeedDegrees, deltaTime);
            }
        }
    }
}
