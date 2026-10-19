using Entropa.Core.StateMachine;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class MoveState : IState<CharacterBrain>
    {
        public void Enter(CharacterBrain context)
        {
        }

        public void Tick(CharacterBrain context, float deltaTime)
        {
            CharacterConfig config = context.Config;
            Vector3 moveDirection = context.CurrentIntent.MoveDirection;
            float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);

            if (inputMagnitude < 0.01f)
            {
                context.ChangeState(context.Idle);
                return;
            }

            if (context.TryStartRoll())
            {
                return;
            }

            if (context.TryStartAttack())
            {
                return;
            }

            bool sprinting = context.CurrentIntent.SprintHeld && inputMagnitude > 0.5f;

            if (sprinting)
            {
                sprinting = context.Stamina.TryDrain(config.SprintDrainPerSecond * deltaTime);
            }

            float speed = sprinting
                ? config.SprintSpeed
                : Mathf.Lerp(config.WalkSpeed, config.RunSpeed, inputMagnitude);

            context.Motor.SetTargetPlanarVelocity(moveDirection.normalized * speed);

            if (context.Targeting.HasTarget)
            {
                Vector3 toTarget = context.Targeting.TargetPoint - context.transform.position;
                context.Motor.FaceDirection(toTarget, config.LockOnTurnSpeedDegrees, deltaTime);
            }
            else
            {
                context.Motor.FaceDirection(moveDirection, config.TurnSpeedDegrees, deltaTime);
            }

            context.Animator.SetLocomotion(speed / config.SprintSpeed);
        }

        public void Exit(CharacterBrain context)
        {
        }
    }
}
