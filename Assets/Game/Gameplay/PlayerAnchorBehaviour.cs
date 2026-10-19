using Entropa.World;
using UnityEngine;

namespace Entropa.Gameplay
{
    public sealed class PlayerAnchorBehaviour : MonoBehaviour, IPlayerAnchor
    {
        private CharacterController _controller;

        public Transform Root => transform;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            bool controllerWasEnabled = _controller != null && _controller.enabled;
            if (controllerWasEnabled)
            {
                _controller.enabled = false;
            }

            transform.SetPositionAndRotation(position, rotation);

            if (controllerWasEnabled)
            {
                _controller.enabled = true;
            }
        }
    }
}
