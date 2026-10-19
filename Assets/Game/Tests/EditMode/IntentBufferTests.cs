using Entropa.Core;
using NUnit.Framework;

namespace Entropa.Tests.EditMode
{
    public sealed class IntentBufferTests
    {
        [Test]
        public void FreshBuffer_DoesNotConsume()
        {
            var buffer = new IntentBuffer();

            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.Roll, 10f, 0.35f));
        }

        [Test]
        public void Press_WithinWindow_IsConsumed()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.Roll, 10f);

            Assert.IsTrue(buffer.ConsumeIfPressedWithin(CharacterAction.Roll, 10.2f, 0.35f));
        }

        [Test]
        public void ConsumedPress_IsNotConsumedTwice()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.HeavyAttack, 5f);

            Assert.IsTrue(buffer.ConsumeIfPressedWithin(CharacterAction.HeavyAttack, 5.1f, 0.35f));
            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.HeavyAttack, 5.1f, 0.35f));
        }

        [Test]
        public void Press_OlderThanWindow_IsRejected()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.LightAttack, 10f);

            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.LightAttack, 10.36f, 0.35f));
        }

        [Test]
        public void Press_ExactlyAtWindowEdge_IsAccepted()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.Roll, 10f);

            Assert.IsTrue(buffer.ConsumeIfPressedWithin(CharacterAction.Roll, 10.25f, 0.25f));
        }

        [Test]
        public void Actions_AreTrackedIndependently()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.Roll, 3f);

            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.LightAttack, 3f, 0.35f));
            Assert.IsTrue(buffer.ConsumeIfPressedWithin(CharacterAction.Roll, 3f, 0.35f));
        }

        [Test]
        public void Clear_DropsAllPendingPresses()
        {
            var buffer = new IntentBuffer();
            buffer.NotifyPressed(CharacterAction.Roll, 1f);
            buffer.NotifyPressed(CharacterAction.LightAttack, 1f);
            buffer.NotifyPressed(CharacterAction.HeavyAttack, 1f);

            buffer.Clear();

            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.Roll, 1f, 0.35f));
            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.LightAttack, 1f, 0.35f));
            Assert.IsFalse(buffer.ConsumeIfPressedWithin(CharacterAction.HeavyAttack, 1f, 0.35f));
        }
    }
}
