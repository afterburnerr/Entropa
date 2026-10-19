using Entropa.Cameras;
using Entropa.Character;
using Entropa.Core;
using Entropa.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entropa.Gameplay
{
    [RequireComponent(typeof(CharacterBrain))]
    public sealed class PlayerBrain : MonoBehaviour, IIntentSource
    {
        [SerializeField] private InputActionAsset _controlsAsset;
        [SerializeField] private CameraRig _cameraRig;
        [SerializeField] private float _gamepadTargetSwitchThreshold = 0.7f;

        private CharacterBrain _brain;
        private InputActionAsset _runtimeControlsAsset;
        private CharacterControls _controls;
        private float _nextTargetSwitchTime;

        public CharacterControls Controls => _controls;

        private void Awake()
        {
            _brain = GetComponent<CharacterBrain>();

            if (_controlsAsset == null)
            {
                Debug.LogError($"{name}: InputActionAsset is not assigned to {nameof(PlayerBrain)}.", this);
                enabled = false;
                return;
            }

            _runtimeControlsAsset = Instantiate(_controlsAsset);
            _controls = new CharacterControls(_runtimeControlsAsset);
            _controls.Enable();
        }

        private void OnDestroy()
        {
            if (_controls != null)
            {
                _controls.Dispose();
            }

            if (_runtimeControlsAsset != null)
            {
                Destroy(_runtimeControlsAsset);
            }
        }

        public CharacterIntent GetIntent(float deltaTime)
        {
            if (_brain.IsDead)
            {
                return default;
            }

            if (_controls.LockOnPressedThisFrame)
            {
                _brain.Targeting.ToggleTarget();
            }

            HandleTargetSwitch();

            Vector2 lookInput = _controls.Look;

            if (_cameraRig != null)
            {
                _cameraRig.ApplyLookInput(lookInput, _controls.LookInputFromGamepad);
            }

            return new CharacterIntent
            {
                MoveDirection = ToWorldDirection(_controls.Move),
                SprintHeld = _controls.SprintHeld,
                RollPressed = _controls.RollPressedThisFrame,
                LightAttackPressed = _controls.LightAttackPressedThisFrame,
                HeavyAttackPressed = _controls.HeavyAttackPressedThisFrame
            };
        }

        private Vector3 ToWorldDirection(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            Vector3 forward = _cameraRig != null ? _cameraRig.PlanarForward : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 world = forward * moveInput.y + right * moveInput.x;

            if (world.sqrMagnitude > 1f)
            {
                world.Normalize();
            }

            return world;
        }

        private void HandleTargetSwitch()
        {
            if (Time.time < _nextTargetSwitchTime || !_brain.Targeting.HasTarget)
            {
                return;
            }

            int direction = 0;
            float switchInput = _controls.TargetSwitchInput;

            if (Mathf.Abs(switchInput) > 0.01f)
            {
                direction = switchInput > 0f ? 1 : -1;
            }
            else if (_controls.LookInputFromGamepad && Mathf.Abs(_controls.Look.x) > _gamepadTargetSwitchThreshold)
            {
                direction = _controls.Look.x > 0f ? 1 : -1;
            }

            if (direction != 0 && _brain.Targeting.SwitchTarget(direction))
            {
                _nextTargetSwitchTime = Time.time + _brain.Config.TargetSwitchCooldown;
            }
        }
    }
}
