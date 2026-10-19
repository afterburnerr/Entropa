using System.Collections;
using System.Reflection;
using Entropa.Character;
using Entropa.Combat;
using Entropa.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Entropa.Tests.PlayMode
{
    public sealed class CharacterMovementTests
    {
        private sealed class TestIntentSource : MonoBehaviour, IIntentSource
        {
            public CharacterIntent Next { get; set; }

            public CharacterIntent GetIntent(float deltaTime)
            {
                return Next;
            }
        }

        private GameObject _player;
        private GameObject _ground;
        private TestIntentSource _source;
        private CharacterConfig _config;
        private MovesetDefinition _moveset;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _ground.layer = GameLayers.Environment;
            _ground.transform.position = Vector3.zero;
            _ground.transform.localScale = new Vector3(3f, 1f, 3f);

            _config = ScriptableObject.CreateInstance<CharacterConfig>();
            _moveset = ScriptableObject.CreateInstance<MovesetDefinition>();

            _player = BuildPlayer(new Vector3(0f, 0.2f, 0f));
            _source.Next = new CharacterIntent { MoveDirection = Vector3.forward };

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_player);
            Object.Destroy(_ground);
            Object.Destroy(_config);
            Object.Destroy(_moveset);
            yield return null;
        }

        private GameObject BuildPlayer(Vector3 position)
        {
            var player = new GameObject("TestPlayer");
            player.SetActive(false);
            player.layer = GameLayers.Player;
            player.transform.position = position;

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.skinWidth = 0.02f;

            var motor = player.AddComponent<CharacterMotor>();
            var stamina = player.AddComponent<CharacterStamina>();
            player.AddComponent<Health>();
            player.AddComponent<Hurtbox>();
            player.AddComponent<InvulnerabilityController>();
            var targeting = player.AddComponent<CharacterTargeting>();
            player.AddComponent<HitboxEmitter>();
            var animator = player.AddComponent<PlaceholderCharacterAnimator>();
            _source = player.AddComponent<TestIntentSource>();
            var brain = player.AddComponent<CharacterBrain>();

            SetReference(motor, "_config", _config);
            SetReference(stamina, "_config", _config);
            SetReference(targeting, "_config", _config);
            SetReference(targeting, "_viewSource", player.transform);
            SetReference(animator, "_visual", player.transform);
            SetReference(brain, "_config", _config);
            SetReference(brain, "_moveset", _moveset);
            SetReference(brain, "_intentSourceBehaviour", _source);
            SetReference(brain, "_animatorBehaviour", animator);

            player.SetActive(true);
            return player;
        }

        private static void SetReference(Component target, string field, Object value)
        {
            System.Type type = target.GetType();

            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(
                    field,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(target, value);
                    return;
                }

                type = type.BaseType;
            }

            Assert.Fail($"field '{field}' not found on {target.GetType().Name}");
        }

        private static IEnumerator RunForSeconds(float seconds)
        {
            float elapsed = 0f;
            int frames = 0;

            while (elapsed < seconds && frames < 900)
            {
                elapsed += Time.deltaTime;
                frames++;
                yield return null;
            }
        }

        [Test]
        public void Intent_FlowsIntoBrain()
        {
            var brain = _player.GetComponent<CharacterBrain>();

            Assert.AreEqual(1f, brain.CurrentIntent.MoveDirection.z, 0.001f);
        }

        [UnityTest]
        public IEnumerator Player_MovesForward_UnderIntent_AndStaysGrounded()
        {
            float startZ = _player.transform.position.z;

            yield return RunForSeconds(1.5f);

            float traveled = _player.transform.position.z - startZ;
            Assert.Greater(traveled, 0.5f, $"expected forward travel, got {traveled:0.00}");

            float height = _player.transform.position.y;
            Assert.That(height, Is.InRange(-0.1f, 0.3f), "player should stay on the ground plane");
        }

        [UnityTest]
        public IEnumerator Player_Idles_WhenIntentIsEmpty()
        {
            _source.Next = new CharacterIntent();
            yield return null;

            Vector3 before = _player.transform.position;
            yield return RunForSeconds(0.75f);

            Vector3 delta = _player.transform.position - before;
            delta.y = 0f;
            Assert.Less(delta.magnitude, 0.1f, "empty intent must not produce planar motion");
        }
    }
}
