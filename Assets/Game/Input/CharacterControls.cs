using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entropa.Input
{
    public sealed class CharacterControls : IDisposable
    {
        public const string MapName = "Character";
        public const string MoveName = "Move";
        public const string LookName = "Look";
        public const string SprintName = "Sprint";
        public const string WalkName = "Walk";
        public const string RollName = "Roll";
        public const string LightAttackName = "LightAttack";
        public const string HeavyAttackName = "HeavyAttack";
        public const string BlockName = "Block";
        public const string LockOnName = "LockOn";
        public const string SwitchTargetName = "SwitchTarget";

        public static readonly string[] ActionNames =
        {
            MoveName,
            LookName,
            SprintName,
            WalkName,
            RollName,
            LightAttackName,
            HeavyAttackName,
            BlockName,
            LockOnName,
            SwitchTargetName
        };

        private readonly InputActionMap _map;
        private readonly InputAction _move;
        private readonly InputAction _look;
        private readonly InputAction _sprint;
        private readonly InputAction _walk;
        private readonly InputAction _roll;
        private readonly InputAction _lightAttack;
        private readonly InputAction _heavyAttack;
        private readonly InputAction _block;
        private readonly InputAction _lockOn;
        private readonly InputAction _switchTarget;

        public CharacterControls(InputActionAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            _map = asset.FindActionMap(MapName, throwIfNotFound: true);
            _move = FindAction(MoveName);
            _look = FindAction(LookName);
            _sprint = FindAction(SprintName);
            _walk = FindAction(WalkName);
            _roll = FindAction(RollName);
            _lightAttack = FindAction(LightAttackName);
            _heavyAttack = FindAction(HeavyAttackName);
            _block = FindAction(BlockName);
            _lockOn = FindAction(LockOnName);
            _switchTarget = FindAction(SwitchTargetName);
        }

        public Vector2 Move => _move.ReadValue<Vector2>();

        public Vector2 Look => _look.ReadValue<Vector2>();

        public bool SprintHeld => _sprint.IsPressed();

        public bool WalkHeld => _walk.IsPressed();

        public bool RollPressedThisFrame => _roll.WasPressedThisFrame();

        public bool LightAttackPressedThisFrame => _lightAttack.WasPressedThisFrame();

        public bool HeavyAttackPressedThisFrame => _heavyAttack.WasPressedThisFrame();

        public bool BlockHeld => _block.IsPressed();

        public bool LockOnPressedThisFrame => _lockOn.WasPressedThisFrame();

        public float TargetSwitchInput => _switchTarget.ReadValue<float>();

        public bool LookInputFromGamepad => _look.activeControl != null && _look.activeControl.device is Gamepad;

        public void Enable()
        {
            _map.Enable();
        }

        public void Disable()
        {
            _map.Disable();
        }

        public void Dispose()
        {
            Disable();
        }

        private InputAction FindAction(string actionName)
        {
            return _map.FindAction(actionName, throwIfNotFound: true);
        }
    }
}
