using System;
using System.Collections.Generic;
using Entropa.Core.StateMachine;
using NUnit.Framework;

namespace Entropa.Tests.EditMode
{
    public sealed class StateMachineTests
    {
        private sealed class Journal
        {
            public readonly List<string> Entries = new List<string>();
        }

        private sealed class RecordingState : IState<Journal>
        {
            private readonly string _name;

            public RecordingState(string name)
            {
                _name = name;
            }

            public void Enter(Journal context)
            {
                context.Entries.Add("enter:" + _name);
            }

            public void Tick(Journal context, float deltaTime)
            {
                context.Entries.Add("tick:" + _name);
            }

            public void Exit(Journal context)
            {
                context.Entries.Add("exit:" + _name);
            }
        }

        [Test]
        public void ChangeState_EntersTheNewState()
        {
            var journal = new Journal();
            var machine = new StateMachine<Journal>(journal);
            var idle = new RecordingState("idle");

            machine.ChangeState(idle);

            Assert.AreSame(idle, machine.Current);
            CollectionAssert.AreEqual(new[] { "enter:idle" }, journal.Entries);
        }

        [Test]
        public void ChangeState_ExitsPreviousStateBeforeEnteringNext()
        {
            var journal = new Journal();
            var machine = new StateMachine<Journal>(journal);
            machine.ChangeState(new RecordingState("idle"));
            machine.ChangeState(new RecordingState("move"));

            CollectionAssert.AreEqual(
                new[] { "enter:idle", "exit:idle", "enter:move" },
                journal.Entries);
        }

        [Test]
        public void ChangeState_ToSameInstance_IsIgnored()
        {
            var journal = new Journal();
            var machine = new StateMachine<Journal>(journal);
            var idle = new RecordingState("idle");
            machine.ChangeState(idle);
            machine.ChangeState(idle);

            CollectionAssert.AreEqual(new[] { "enter:idle" }, journal.Entries);
        }

        [Test]
        public void ChangeState_Null_Throws()
        {
            var machine = new StateMachine<Journal>(new Journal());

            Assert.Throws<ArgumentNullException>(() => machine.ChangeState(null));
        }

        [Test]
        public void Tick_WithoutCurrentState_DoesNothing()
        {
            var machine = new StateMachine<Journal>(new Journal());

            Assert.DoesNotThrow(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_IsForwardedToCurrentState()
        {
            var journal = new Journal();
            var machine = new StateMachine<Journal>(journal);
            machine.ChangeState(new RecordingState("idle"));

            machine.Tick(0.02f);

            CollectionAssert.AreEqual(new[] { "enter:idle", "tick:idle" }, journal.Entries);
        }
    }
}
