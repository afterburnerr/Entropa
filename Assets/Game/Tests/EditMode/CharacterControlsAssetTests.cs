using Entropa.Input;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Entropa.Tests.EditMode
{
    public sealed class CharacterControlsAssetTests
    {
        private const string AssetPath = "Assets/Game/Input/CharacterControls.inputactions";

        private InputActionAsset _asset;
        private InputActionMap _map;

        [SetUp]
        public void SetUp()
        {
            _asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            Assert.IsNotNull(_asset, $"input actions asset missing at {AssetPath}");
            _map = _asset.FindActionMap(CharacterControls.MapName, false);
        }

        [TearDown]
        public void TearDown()
        {
            _asset = null;
            _map = null;
        }

        [Test]
        public void CharacterMap_Exists()
        {
            Assert.IsNotNull(_map, $"map '{CharacterControls.MapName}' missing");
        }

        [Test]
        public void AllKnownActions_ExistInMap()
        {
            foreach (string actionName in CharacterControls.ActionNames)
            {
                Assert.IsNotNull(_map.FindAction(actionName, false), $"missing action '{actionName}'");
            }
        }

        [Test]
        public void EveryAction_HasAtLeastOneBinding()
        {
            foreach (InputAction action in _map.actions)
            {
                Assert.Greater(action.bindings.Count, 0, $"action '{action.name}' has no bindings");
            }
        }

        [Test]
        public void MoveAndLook_AreVector2ValueActions()
        {
            foreach (string name in new[] { CharacterControls.MoveName, CharacterControls.LookName })
            {
                InputAction action = _map.FindAction(name, true);
                Assert.AreEqual(InputActionType.Value, action.type, $"'{name}' must be a Value action");
                Assert.AreEqual("Vector2", action.expectedControlType, $"'{name}' must be Vector2");
            }
        }

        [Test]
        public void CoreActions_SupportTheirInputDevices()
        {
            AssertActionHasDevice(CharacterControls.MoveName, "<Keyboard>");
            AssertActionHasDevice(CharacterControls.MoveName, "<Gamepad>");
            AssertActionHasDevice(CharacterControls.LookName, "<Mouse>");
            AssertActionHasDevice(CharacterControls.LookName, "<Gamepad>");
            AssertActionHasDevice(CharacterControls.SprintName, "<Keyboard>");
            AssertActionHasDevice(CharacterControls.SprintName, "<Gamepad>");
            AssertActionHasDevice(CharacterControls.WalkName, "<Keyboard>");
            AssertActionHasDevice(CharacterControls.RollName, "<Keyboard>");
            AssertActionHasDevice(CharacterControls.RollName, "<Gamepad>");
        }

        private void AssertActionHasDevice(string actionName, string devicePrefix)
        {
            InputAction action = _map.FindAction(actionName, true);

            foreach (InputBinding binding in action.bindings)
            {
                if (binding.effectivePath.StartsWith(devicePrefix))
                {
                    return;
                }
            }

            Assert.Fail($"'{actionName}' lacks a binding starting with {devicePrefix}");
        }

        [Test]
        public void Wrapper_CanBeConstructedAndDisposed()
        {
            using (var controls = new CharacterControls(_asset))
            {
                controls.Enable();
                controls.Disable();
            }
        }
    }
}
