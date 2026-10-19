using UnityEngine;

namespace Entropa.Core
{
    public struct CharacterIntent
    {
        public Vector3 MoveDirection;
        public bool SprintHeld;
        public bool WalkHeld;
        public bool RollPressed;
        public bool LightAttackPressed;
        public bool HeavyAttackPressed;
    }
}
