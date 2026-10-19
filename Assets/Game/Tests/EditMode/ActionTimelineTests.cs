using Entropa.Character;
using NUnit.Framework;

namespace Entropa.Tests.EditMode
{
    public sealed class ActionTimelineTests
    {
        private static ActionTimeline CreateTimeline()
        {
            return new ActionTimeline(new ActionTiming(0.25f, 0.15f, 0.35f, 0.5f));
        }

        [Test]
        public void StartsInStartup()
        {
            var timeline = CreateTimeline();

            Assert.IsTrue(timeline.IsInStartup);
            Assert.IsFalse(timeline.IsActive);
            Assert.IsFalse(timeline.IsFinished);
            Assert.AreEqual(0f, timeline.NormalizedTime);
        }

        [Test]
        public void Advance_CrossesIntoActive()
        {
            var timeline = CreateTimeline();

            timeline.Advance(0.25f);

            Assert.IsFalse(timeline.IsInStartup);
            Assert.IsTrue(timeline.IsActive);
        }

        [Test]
        public void Advance_CrossesIntoRecovery()
        {
            var timeline = CreateTimeline();

            timeline.Advance(0.4f);

            Assert.IsTrue(timeline.IsInRecovery);
            Assert.IsFalse(timeline.IsFinished);
        }

        [Test]
        public void Advance_CompletesTimeline()
        {
            var timeline = CreateTimeline();

            timeline.Advance(0.75f);

            Assert.IsTrue(timeline.IsFinished);
            Assert.AreEqual(1f, timeline.NormalizedTime);
        }

        [Test]
        public void CancelWindow_OpensAtCancelStart()
        {
            var timeline = CreateTimeline();

            timeline.Advance(0.49f);
            Assert.IsFalse(timeline.IsCancelWindowOpen);

            timeline.Advance(0.01f);
            Assert.IsTrue(timeline.IsCancelWindowOpen);
        }

        [Test]
        public void Advance_NonPositiveDelta_IsIgnored()
        {
            var timeline = CreateTimeline();
            timeline.Advance(0.2f);

            timeline.Advance(0f);
            timeline.Advance(-1f);

            Assert.AreEqual(0.2f, timeline.Elapsed, 0.0001f);
        }

        [Test]
        public void ZeroDuration_IsFinishedImmediately()
        {
            var timeline = new ActionTimeline(new ActionTiming(0f, 0f, 0f, 0f));

            Assert.IsTrue(timeline.IsFinished);
            Assert.AreEqual(1f, timeline.NormalizedTime);
        }
    }
}
