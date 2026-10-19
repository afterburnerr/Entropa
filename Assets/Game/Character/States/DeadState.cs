using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class DeadState : IState<CharacterBrain>
    {
        public void Enter(CharacterBrain context)
        {
            context.Motor.SetTargetPlanarVelocity(Vector3.zero);
            context.Targeting.ClearTarget();
            context.Animator.PlayDeath();
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            context.Motor.SetTargetPlanarVelocity(Vector3.zero);
        }

        public void Exit(CharacterBrain context)
        {
        }
    }
}
