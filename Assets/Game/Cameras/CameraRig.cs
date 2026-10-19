using Entropa.Core;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Entropa.Cameras
{
    public sealed class CameraRig : MonoBehaviour
    {
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
        [SerializeField] private LayerMask _collisionMask = GameLayers.EnvironmentMask;

        private Transform _followAnchor;
        private ITargetProvider _targetProvider;
        private CinemachineCamera _virtualCamera;
        private CinemachineOrbitalFollow _orbital;
        private CinemachineDeoccluder _deoccluder;

        public Camera ViewCamera { get; private set; }

        public Vector3 PlanarForward => Quaternion.Euler(0f, _orbital.HorizontalAxis.Value, 0f) * Vector3.forward;

        [Inject]
        public void Construct(CameraAnchor followAnchor, ITargetProvider targetProvider)
        {
            _followAnchor = followAnchor.transform;
            _targetProvider = targetProvider;
        }

        private void Start()
        {
            ViewCamera = GetComponentInChildren<Camera>();
            _virtualCamera = GetComponentInChildren<CinemachineCamera>();

            if (ViewCamera == null)
            {
                Debug.LogError($"{name}: no Camera found under {nameof(CameraRig)}.", this);
                enabled = false;
                return;
            }

            if (_virtualCamera == null)
            {
                Debug.LogError($"{name}: no {nameof(CinemachineCamera)} found under {nameof(CameraRig)}.", this);
                enabled = false;
                return;
            }

            _orbital = _virtualCamera.GetComponent<CinemachineOrbitalFollow>();
            _deoccluder = _virtualCamera.GetComponent<CinemachineDeoccluder>();

            if (_orbital == null)
            {
                Debug.LogError($"{name}: {nameof(CinemachineOrbitalFollow)} is required on {_virtualCamera.name}.", this);
                enabled = false;
                return;
            }

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

            _virtualCamera.Follow = _followAnchor;
            _virtualCamera.LookAt = _followAnchor;
            _virtualCamera.Lens = LensSettings.FromCamera(ViewCamera);

            _orbital.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            _orbital.Radius = _distance;
            _orbital.TargetOffset = Vector3.zero;
            _orbital.TrackerSettings.PositionDamping = Vector3.one * _positionSmoothing;

            _orbital.HorizontalAxis.Wrap = true;
            _orbital.HorizontalAxis.Recentering.Enabled = false;
            _orbital.HorizontalAxis.Value = NormalizeAngle(transform.eulerAngles.y);

            _orbital.VerticalAxis.Wrap = false;
            _orbital.VerticalAxis.Recentering.Enabled = false;
            _orbital.VerticalAxis.Range = new Vector2(_minPitch, _maxPitch);
            _orbital.VerticalAxis.Value = Mathf.Clamp(NormalizeAngle(transform.eulerAngles.x), _minPitch, _maxPitch);

            if (_deoccluder != null)
            {
                _deoccluder.CollideAgainst = _collisionMask;
                _deoccluder.AvoidObstacles.Enabled = true;
                _deoccluder.AvoidObstacles.CameraRadius = _collisionRadius;
            }
        }

        public void ApplyLookInput(Vector2 lookDelta, bool isRateInput)
        {
            if (_orbital == null)
            {
                return;
            }

            float scale = isRateInput ? _gamepadLookRate * Time.deltaTime : 1f;
            _orbital.HorizontalAxis.Value += lookDelta.x * _lookSensitivity * scale;
            float pitch = _orbital.VerticalAxis.Value - lookDelta.y * _lookSensitivity * scale;
            _orbital.VerticalAxis.Value = Mathf.Clamp(pitch, _minPitch, _maxPitch);
        }

        private void LateUpdate()
        {
            if (_orbital == null)
            {
                return;
            }

            bool lockedOn = _targetProvider.HasTarget && _targetProvider.TargetTransform != null;
            Vector3 pivot = _followAnchor.position;

            if (!lockedOn)
            {
                _orbital.TargetOffset = Vector3.zero;
                _orbital.Radius = _distance;
                return;
            }

            Vector3 toTarget = _targetProvider.TargetPoint - pivot;
            UpdateLockedRotation(toTarget);

            Vector3 pivotOffset = toTarget * _lockOnPivotBias;
            pivotOffset.y = _lockOnHeightOffset;
            _orbital.TargetOffset = pivotOffset;
            _orbital.Radius = _lockOnDistance;
        }

        private void UpdateLockedRotation(Vector3 toTarget)
        {
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion desired = Quaternion.LookRotation(toTarget, Vector3.up);
            float rate = 1f - Mathf.Exp(-_lockOnRotationRate * Time.deltaTime);
            _orbital.HorizontalAxis.Value = Mathf.LerpAngle(_orbital.HorizontalAxis.Value, desired.eulerAngles.y, rate);
            float desiredPitch = Mathf.Clamp(NormalizeAngle(desired.eulerAngles.x), _minPitch, _maxPitch);
            _orbital.VerticalAxis.Value = Mathf.LerpAngle(_orbital.VerticalAxis.Value, desiredPitch, rate);
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
