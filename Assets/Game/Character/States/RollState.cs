using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class RollState : IState<CharacterBrain>
    {
        private ActionTimeline _timeline;
        private Vector3 _direction;
        private bool _invulnerabilityOpen;

        public void Enter(CharacterBrain context)
        {
            Vector3 moveDirection = context.CurrentIntent.MoveDirection;
            _direction = moveDirection.sqrMagnitude > 0.0001f
                ? new Vector3(moveDirection.x, 0f, moveDirection.z).normalized
                : -context.transform.forward;

            context.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
            context.Motor.SetTargetPlanarVelocity(_direction * context.Config.RollSpeed);
            context.Animator.PlayRoll(_direction);
            _timeline = new ActionTimeline(new ActionTiming(0f, context.Config.RollDuration, 0f, 0f));
            _invulnerabilityOpen = false;
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            CharacterConfig config = context.Config;
            _timeline.Advance(deltaTime);

            float speed = config.RollSpeed * config.RollSpeedCurve.Evaluate(_timeline.NormalizedTime);
            context.Motor.SetTargetPlanarVelocity(_direction * speed);

            if (!_invulnerabilityOpen && _timeline.Elapsed >= config.RollInvulnerabilityStart)
            {
                _invulnerabilityOpen = true;
                context.Invulnerability.Open(this);
            }

            if (_invulnerabilityOpen && _timeline.Elapsed >= config.RollInvulnerabilityEnd)
            {
                _invulnerabilityOpen = false;
                context.Invulnerability.Close(this);
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
            if (_invulnerabilityOpen)
            {
                _invulnerabilityOpen = false;
                context.Invulnerability.Close(this);
            }
        }
    }
}
