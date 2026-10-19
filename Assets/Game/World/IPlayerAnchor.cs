using UnityEngine;

namespace Entropa.World
{
    public interface IPlayerAnchor
    {
        Transform Root { get; }

        void Teleport(Vector3 position, Quaternion rotation);
    }
}
