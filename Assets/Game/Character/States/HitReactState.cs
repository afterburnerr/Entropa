using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class HitReactState : IState<CharacterBrain>
    {
        private float _remaining;

        public void Enter(CharacterBrain context)
        {
            _remaining = context.Config.HitReactDuration;
            context.Motor.SetTargetPlanarVelocity(Vector3.zero);
            context.Animator.PlayHitReact();
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            _remaining -= deltaTime;
            context.Motor.SetTargetPlanarVelocity(Vector3.zero);

            if (_remaining <= 0f)
            {
                context.ChangeState(context.Idle);
            }
        }

        public void Exit(CharacterBrain context)
        {
        }
    }
}
