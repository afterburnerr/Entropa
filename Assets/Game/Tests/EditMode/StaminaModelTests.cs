using Entropa.Character;
using NUnit.Framework;

namespace Entropa.Tests.EditMode
{
    public sealed class StaminaModelTests
    {
        private static StaminaModel CreateModel()
        {
            return new StaminaModel(100f, 10f, 0.5f);
        }

        [Test]
        public void StartsFull()
        {
            var model = CreateModel();

            Assert.AreEqual(100f, model.Current);
            Assert.IsFalse(model.IsDepleted);
        }

        [Test]
        public void TrySpend_WithinBudget_Deducts()
        {
            var model = CreateModel();

            Assert.IsTrue(model.TrySpend(30f));
            Assert.AreEqual(70f, model.Current);
        }

        [Test]
        public void TrySpend_BeyondBudget_FailsWithoutDeducting()
        {
            var model = CreateModel();

            Assert.IsFalse(model.TrySpend(130f));
            Assert.AreEqual(100f, model.Current);
        }

        [Test]
        public void TrySpend_NegativeAmount_Fails()
        {
            var model = CreateModel();

            Assert.IsFalse(model.TrySpend(-5f));
            Assert.AreEqual(100f, model.Current);
        }

        [Test]
        public void TrySpend_ExactBalance_ReachesZero()
        {
            var model = CreateModel();

            Assert.IsTrue(model.TrySpend(100f));
            Assert.AreEqual(0f, model.Current);
            Assert.IsTrue(model.IsDepleted);
        }

        [Test]
        public void Regen_DoesNotStartBeforeDelay()
        {
            var model = CreateModel();
            model.TrySpend(50f);

            model.Tick(0.4f);

            Assert.AreEqual(50f, model.Current);
        }

        [Test]
        public void Regen_StartsAfterDelay()
        {
            var model = CreateModel();
            model.TrySpend(50f);

            model.Tick(0.6f);

            Assert.AreEqual(56f, model.Current);
        }

        [Test]
        public void Regen_ClampsAtMax()
        {
            var model = CreateModel();
            model.TrySpend(10f);

            model.Tick(10f);

            Assert.AreEqual(100f, model.Current);
        }

        [Test]
        public void Spend_ResetsRegenDelay()
        {
            var model = CreateModel();
            model.TrySpend(50f);
            model.Tick(0.4f);
            model.TrySpend(10f);

            model.Tick(0.4f);

            Assert.AreEqual(40f, model.Current);
        }

        [Test]
        public void TryDrain_ReducesWithoutDepleting()
        {
            var model = CreateModel();

            Assert.IsTrue(model.TryDrain(30f));
            Assert.AreEqual(70f, model.Current);
        }

        [Test]
        public void TryDrain_ToZero_ReturnsFalseAndReportsDepleted()
        {
            var model = CreateModel();

            Assert.IsFalse(model.TryDrain(100f));
            Assert.AreEqual(0f, model.Current);
            Assert.IsTrue(model.IsDepleted);
        }

        [Test]
        public void TryDrain_FromEmpty_Fails()
        {
            var model = CreateModel();
            model.TrySpend(100f);

            Assert.IsFalse(model.TryDrain(5f));
        }

        [Test]
        public void TryDrain_NonPositiveAmount_Succeeds()
        {
            var model = CreateModel();
            model.TrySpend(100f);

            Assert.IsTrue(model.TryDrain(0f));
        }

        [Test]
        public void Tick_NonPositiveDelta_IsIgnored()
        {
            var model = CreateModel();
            model.TrySpend(50f);

            model.Tick(0f);
            model.Tick(-1f);

            Assert.AreEqual(50f, model.Current);
        }
    }
}
