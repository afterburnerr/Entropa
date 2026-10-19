using Entropa.Core;
using UnityEngine;

namespace Entropa.Character
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterMotor : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _config;

        private CharacterController _controller;
        private Vector3 _planarVelocity;
        private Vector3 _targetPlanarVelocity;
        private float _verticalVelocity;

        public bool IsGrounded => _controller.isGrounded;

        public Vector3 PlanarVelocity => _planarVelocity;

        public float PlanarSpeed => _planarVelocity.magnitude;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _controller.excludeLayers = ~GameLayers.CharacterCollisionMask;

            if (_config == null)
            {
                Debug.LogError($"{name}: CharacterConfig is not assigned to {nameof(CharacterMotor)}.", this);
                enabled = false;
            }
        }

        public void SetTargetPlanarVelocity(Vector3 velocity)
        {
            velocity.y = 0f;
            _targetPlanarVelocity = velocity;
        }

        public void FaceDirection(Vector3 worldDirection, float degreesPerSecond, float deltaTime)
        {
            Vector3 planar = new Vector3(worldDirection.x, 0f, worldDirection.z);

            if (planar.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(planar.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, degreesPerSecond * deltaTime);
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            float rate = _targetPlanarVelocity.sqrMagnitude >= _planarVelocity.sqrMagnitude
                ? _config.Acceleration
                : _config.Deceleration;

            _planarVelocity = Vector3.MoveTowards(_planarVelocity, _targetPlanarVelocity, rate * deltaTime);

            if (_controller.isGrounded && _verticalVelocity <= 0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += _config.Gravity * deltaTime;
            }

            Vector3 motion = (_planarVelocity + Vector3.up * _verticalVelocity) * deltaTime;
            _controller.Move(motion);
        }
    }
}
