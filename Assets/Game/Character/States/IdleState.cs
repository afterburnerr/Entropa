using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class IdleState : IState<CharacterBrain>
    {
        public void Enter(CharacterBrain context)
        {
            context.Motor.SetTargetPlanarVelocity(Vector3.zero);
            context.Animator.SetLocomotion(0f);
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            if (context.TryStartRoll())
            {
                return;
            }

            if (context.TryStartAttack())
            {
                return;
            }

            if (context.CurrentIntent.MoveDirection.sqrMagnitude > 0.0001f)
            {
                context.ChangeState(context.Move);
                return;
            }

            context.Motor.SetTargetPlanarVelocity(Vector3.zero);
        }

        public void Exit(CharacterBrain context)
        {
        }
    }
}
