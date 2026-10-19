using System.Collections.Generic;
using UnityEngine;

namespace Entropa.Combat
{
    public static class TargetRegistry
    {
        private static readonly List<ITargetable> Targets = new List<ITargetable>();

        public static IReadOnlyList<ITargetable> Active => Targets;

        public static void Register(ITargetable target)
        {
            if (target == null || Targets.Contains(target))
            {
                return;
            }

            Targets.Add(target);
        }

        public static void Unregister(ITargetable target)
        {
            if (target == null)
            {
                return;
            }

            Targets.Remove(target);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            Targets.Clear();
        }
    }
}
