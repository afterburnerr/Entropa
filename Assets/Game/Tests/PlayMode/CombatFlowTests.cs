using System.Collections;
using Entropa.Combat;
using Entropa.Core;
using Entropa.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Entropa.Tests.PlayMode
{
    public sealed class CombatFlowTests
    {
        private GameObject _dummy;
        private Health _health;
        private Hurtbox _hurtbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _dummy = new GameObject("TestDummy");
            _dummy.SetActive(false);
            _dummy.layer = GameLayers.Enemy;

            var collider = _dummy.AddComponent<CapsuleCollider>();
            collider.height = 1.8f;
            collider.radius = 0.4f;
            collider.center = new Vector3(0f, 0.9f, 0f);

            _health = _dummy.AddComponent<Health>();
            _hurtbox = _dummy.AddComponent<Hurtbox>();
            _dummy.AddComponent<TrainingDummy>();

            _dummy.SetActive(true);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_dummy);
            yield return null;
        }

        [Test]
        public void Damage_ReducesHealth_AndRaisesDamaged()
        {
            int damagedCount = 0;
            _health.Damaged += _ => damagedCount++;

            bool applied = _hurtbox.TryReceiveDamage(new DamageInfo { Amount = 30f });

            Assert.IsTrue(applied);
            Assert.AreEqual(70f, _health.Current, 0.001f);
            Assert.AreEqual(1, damagedCount);
        }

        [Test]
        public void Damage_BeyondHealth_KillsOnce_AndSealsHurtbox()
        {
            int diedCount = 0;
            _health.Died += () => diedCount++;

            Assert.IsTrue(_hurtbox.TryReceiveDamage(new DamageInfo { Amount = 120f }));
            Assert.IsFalse(_health.IsAlive);
            Assert.AreEqual(0f, _health.Current, 0.001f);
            Assert.AreEqual(1, diedCount);

            Assert.IsFalse(
                _hurtbox.TryReceiveDamage(new DamageInfo { Amount = 10f }),
                "a dead target must reject further damage");
            Assert.AreEqual(1, diedCount);
        }

        [Test]
        public void ZeroDamage_LeavesHealthUntouched()
        {
            bool applied = _hurtbox.TryReceiveDamage(new DamageInfo { Amount = 0f });

            Assert.IsTrue(applied);
            Assert.AreEqual(100f, _health.Current, 0.001f);
            Assert.IsTrue(_health.IsAlive);
        }

        [UnityTest]
        public IEnumerator Hurtbox_IsTargetable_WhileAlive_AndNotAfterDeath()
        {
            Assert.IsTrue(_hurtbox.IsTargetable);
            Assert.AreEqual(_dummy.transform, _hurtbox.TargetTransform);

            _hurtbox.TryReceiveDamage(new DamageInfo { Amount = 100f });
            yield return null;

            Assert.IsFalse(_hurtbox.IsTargetable);
            Assert.IsFalse(_hurtbox.TryReceiveDamage(new DamageInfo { Amount = 5f }));
        }
    }
}
