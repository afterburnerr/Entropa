using UnityEngine;

namespace Entropa.Character
{
    public interface ICharacterAnimator
    {
        void SetLocomotion(float normalizedSpeed);

        void PlayRoll(Vector3 worldDirection);

        void PlayAttack(AttackDefinition attack);

        void PlayHitReact();

        void PlayDeath();
    }
}
