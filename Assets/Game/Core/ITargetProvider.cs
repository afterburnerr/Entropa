using UnityEngine;

namespace Entropa.Core
{
    public interface ITargetProvider
    {
        bool HasTarget { get; }

        Transform TargetTransform { get; }

        Vector3 TargetPoint { get; }
    }
}
