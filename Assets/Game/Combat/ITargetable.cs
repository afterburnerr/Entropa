using UnityEngine;

namespace Entropa.Combat
{
    public interface ITargetable
    {
        bool IsTargetable { get; }

        Transform TargetTransform { get; }

        Vector3 TargetPoint { get; }
    }
}
