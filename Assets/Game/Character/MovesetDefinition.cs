using System.Collections.Generic;
using UnityEngine;

namespace Entropa.Character
{
    [CreateAssetMenu(menuName = "Entropa/Moveset", fileName = "Moveset")]
    public sealed class MovesetDefinition : ScriptableObject
    {
        [SerializeField] private List<AttackDefinition> _lightCombo = new List<AttackDefinition>();
        [SerializeField] private List<AttackDefinition> _heavyCombo = new List<AttackDefinition>();

        public IReadOnlyList<AttackDefinition> LightCombo => _lightCombo;

        public IReadOnlyList<AttackDefinition> HeavyCombo => _heavyCombo;
    }
}
