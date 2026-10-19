using Entropa.Core;
using UnityEngine;

namespace Entropa.Cameras
{
    public sealed class CameraRig : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _followAnchor;
        [SerializeField] private MonoBehaviour _targetProviderBehaviour;

        [Header("Orbit")]
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _lookSensitivity = 1f;
        [SerializeField] private float _gamepadLookRate = 2.5f;
        [SerializeField] private float _minPitch = -35f;
        [SerializeField] private float _maxPitch = 70f;
        [SerializeField] private float _positionSmoothing = 0.06f;
        [SerializeField] private float _lockOnRotationRate = 10f;

        [Header("Lock On")]
        [SerializeField] private float _lockOnDistance = 6f;
        [SerializeField] private float _lockOnPivotBias = 0.33f;
        [SerializeField] private float _lockOnHeightOffset = 0.4f;

        [Header("Collision")]
        [SerializeField] private float _collisionRadius = 0.25f;
        [SerializeField] private float _collisionPullInSpeed = 30f;
        [SerializeField] private float _collisionPushOutSpeed = 4f;
        [SerializeField] private LayerMask _collisionMask = GameLayers.EnvironmentMask;

        private Transform _cameraTransform;
        private ITargetProvider _targetProvider;
        private Vector3 _pivotVelocity;
        private float _yaw;
        private float _pitch;
        private float _currentDistance;
        private bool _hasPosition;

        public Camera ViewCamera { get; private set; }

        public Vector3 PlanarForward => Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;

        private void Awake()
        {
            ViewCamera = GetComponentInChildren<Camera>();

            if (ViewCamera == null)
            {
                Debug.LogError($"{name}: no Camera found under {nameof(CameraRig)}.", this);
                enabled = false;
                return;
            }

            _cameraTransform = ViewCamera.transform;
            _targetProvider = _targetProviderBehaviour as ITargetProvider;

            if (_targetProvider == null)
            {
                Debug.LogError($"{name}: target provider must implement {nameof(ITargetProvider)}.", this);
                enabled = false;
                return;
            }

            if (_followAnchor == null)
            {
                Debug.LogError($"{name}: follow anchor is not assigned.", this);
                enabled = false;
                return;
            }

            _yaw = transform.eulerAngles.y;
            _pitch = NormalizeAngle(transform.eulerAngles.x);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            _currentDistance = _distance;
        }

        public void ApplyLookInput(Vector2 lookDelta, bool isRateInput)
        {
            float scale = isRateInput ? _gamepadLookRate * Time.deltaTime : 1f;
            _yaw += lookDelta.x * _lookSensitivity * scale;
            _pitch = Mathf.Clamp(_pitch - lookDelta.y * _lookSensitivity * scale, _minPitch, _maxPitch);
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            bool lockedOn = _targetProvider.HasTarget && _targetProvider.TargetTransform != null;
            Vector3 pivotTarget = ComputePivotTarget(lockedOn);

            if (lockedOn)
            {
                UpdateLockedRotation(pivotTarget, deltaTime);
            }

            if (!_hasPosition)
            {
                _hasPosition = true;
                transform.position = pivotTarget;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, pivotTarget, ref _pivotVelocity, _positionSmoothing);
            }

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            float desiredDistance = lockedOn ? _lockOnDistance : _distance;
            float distance = ResolveCollision(desiredDistance, deltaTime);
            _cameraTransform.position = transform.position + transform.rotation * (Vector3.back * distance);
            _cameraTransform.rotation = transform.rotation;
        }

        private Vector3 ComputePivotTarget(bool lockedOn)
        {
            Vector3 pivot = _followAnchor.position;

            if (!lockedOn)
            {
                return pivot;
            }

            Vector3 toTarget = _targetProvider.TargetPoint - pivot;
            Vector3 result = pivot + toTarget * _lockOnPivotBias;
            result.y = pivot.y + _lockOnHeightOffset;
            return result;
        }

        private void UpdateLockedRotation(Vector3 pivot, float deltaTime)
        {
            Vector3 lookDirection = _targetProvider.TargetPoint - pivot;

            if (lookDirection.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion desired = Quaternion.LookRotation(lookDirection, Vector3.up);
            float desiredYaw = desired.eulerAngles.y;
            float desiredPitch = Mathf.Clamp(NormalizeAngle(desired.eulerAngles.x), _minPitch, _maxPitch);
            float rate = 1f - Mathf.Exp(-_lockOnRotationRate * deltaTime);
            _yaw = Mathf.LerpAngle(_yaw, desiredYaw, rate);
            _pitch = Mathf.LerpAngle(_pitch, desiredPitch, rate);
        }

        private float ResolveCollision(float desiredDistance, float deltaTime)
        {
            float targetDistance = desiredDistance;
            Vector3 direction = -(transform.rotation * Vector3.forward);

            if (Physics.SphereCast(
                    transform.position,
                    _collisionRadius,
                    direction,
                    out RaycastHit hit,
                    desiredDistance,
                    _collisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                targetDistance = Mathf.Max(hit.distance, _collisionRadius);
            }

            float speed = targetDistance < _currentDistance ? _collisionPullInSpeed : _collisionPushOutSpeed;
            return Mathf.MoveTowards(_currentDistance, targetDistance, speed * deltaTime);
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
