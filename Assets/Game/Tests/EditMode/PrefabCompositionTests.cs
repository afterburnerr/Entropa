using Entropa.Cameras;
using Entropa.Character;
using Entropa.Combat;
using Entropa.Core;
using Entropa.Gameplay;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Entropa.Tests.EditMode
{
    public sealed class PrefabCompositionTests
    {
        private const string PlayerPath = "Assets/Prefabs/PlayerCharacter.prefab";
        private const string DummyPath = "Assets/Prefabs/TrainingDummy.prefab";
        private const string RigPath = "Assets/Prefabs/CameraRig.prefab";

        private static GameObject LoadPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab, $"prefab missing at {path}");
            return prefab;
        }

        private static Object GetReference(Component component, string field)
        {
            var serialized = new SerializedObject(component);
            SerializedProperty property = serialized.FindProperty(field);
            Assert.IsNotNull(property, $"{component.GetType().Name}.{field} not found");
            return property.objectReferenceValue;
        }

        [Test]
        public void PlayerPrefab_HasAllModules()
        {
            GameObject player = LoadPrefab(PlayerPath);

            Assert.IsNotNull(player.GetComponent<CharacterController>());
            Assert.IsNotNull(player.GetComponent<CharacterMotor>());
            Assert.IsNotNull(player.GetComponent<CharacterStamina>());
            Assert.IsNotNull(player.GetComponent<Health>());
            Assert.IsNotNull(player.GetComponent<Hurtbox>());
            Assert.IsNotNull(player.GetComponent<InvulnerabilityController>());
            Assert.IsNotNull(player.GetComponent<CharacterTargeting>());
            Assert.IsNotNull(player.GetComponent<HitboxEmitter>());
            Assert.IsNotNull(player.GetComponent<CharacterBrain>());
            Assert.IsNotNull(player.GetComponent<PlayerBrain>());
            Assert.IsNotNull(player.GetComponent<PlaceholderCharacterAnimator>());
            Assert.IsNotNull(player.GetComponent<CursorLockController>());
        }

        [Test]
        public void PlayerPrefab_IsOnPlayerLayer_WithAnchorsAndColliderlessVisual()
        {
            GameObject player = LoadPrefab(PlayerPath);

            Assert.AreEqual(GameLayers.Player, player.layer);
            Assert.IsNotNull(player.transform.Find("CameraAnchor"));

            Transform visual = player.transform.Find("Visual");
            Assert.IsNotNull(visual);
            Assert.IsNull(visual.GetComponent<Collider>(), "Visual must not carry a collider");
        }

        [Test]
        public void PlayerPrefab_CharacterController_MatchesMetrics()
        {
            GameObject player = LoadPrefab(PlayerPath);
            var controller = player.GetComponent<CharacterController>();

            Assert.AreEqual(1.8f, controller.height, 0.001f);
            Assert.AreEqual(0.35f, controller.radius, 0.001f);
            Assert.AreEqual(0.9f, controller.center.y, 0.001f);
        }

        [Test]
        public void PlayerPrefab_BrainAndModuleReferences_AreWired()
        {
            GameObject player = LoadPrefab(PlayerPath);
            var brain = player.GetComponent<CharacterBrain>();

            Assert.IsNotNull(GetReference(brain, "_config"));
            Assert.IsNotNull(GetReference(brain, "_moveset"));
            Assert.IsInstanceOf<PlayerBrain>(GetReference(brain, "_intentSourceBehaviour"));
            Assert.IsInstanceOf<PlaceholderCharacterAnimator>(GetReference(brain, "_animatorBehaviour"));
            Assert.IsNotNull(GetReference(player.GetComponent<PlayerBrain>(), "_controlsAsset"));
            Assert.IsNotNull(GetReference(player.GetComponent<CharacterMotor>(), "_config"));
            Assert.IsNotNull(GetReference(player.GetComponent<CharacterStamina>(), "_config"));
            Assert.IsNotNull(GetReference(player.GetComponent<CharacterTargeting>(), "_config"));
            Assert.IsNotNull(GetReference(player.GetComponent<CharacterTargeting>(), "_viewSource"));
        }

        [Test]
        public void DummyPrefab_HasCombatModules_OnEnemyLayer()
        {
            GameObject dummy = LoadPrefab(DummyPath);

            Assert.AreEqual(GameLayers.Enemy, dummy.layer);
            Assert.IsNotNull(dummy.GetComponent<CapsuleCollider>());
            Assert.IsNotNull(dummy.GetComponent<Health>());
            Assert.IsNotNull(dummy.GetComponent<Hurtbox>());
            Assert.IsNotNull(dummy.GetComponent<DamageFlash>());
            Assert.IsNotNull(dummy.GetComponent<TrainingDummy>());
        }

        [Test]
        public void DummyPrefab_HealthIs200_AndWiringComplete()
        {
            GameObject dummy = LoadPrefab(DummyPath);
            var serialized = new SerializedObject(dummy.GetComponent<Health>());

            Assert.AreEqual(200f, serialized.FindProperty("_maxHealth").floatValue, 0.001f);
            Assert.IsNotNull(GetReference(dummy.GetComponent<DamageFlash>(), "_renderer"));
            Assert.IsNotNull(GetReference(dummy.GetComponent<TrainingDummy>(), "_visual"));
        }

        [Test]
        public void CameraRigPrefab_HasTaggedCameraWithAudioListener()
        {
            GameObject rig = LoadPrefab(RigPath);

            Assert.IsNotNull(rig.GetComponent<CameraRig>());

            Transform cameraTransform = rig.transform.Find("Camera");
            Assert.IsNotNull(cameraTransform);
            Assert.AreEqual("MainCamera", cameraTransform.tag);
            Assert.IsNotNull(cameraTransform.GetComponent<Camera>());
            Assert.IsNotNull(cameraTransform.GetComponent<AudioListener>());
        }
    }
}
