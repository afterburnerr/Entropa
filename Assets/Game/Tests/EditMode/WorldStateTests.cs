using System.Collections.Generic;
using Entropa.World;
using NUnit.Framework;
using UnityEngine;

namespace Entropa.Tests.EditMode
{
    [TestFixture]
    public sealed class WorldStateTests
    {
        [Test]
        public void DefaultStateHasNoLocationAndNoFlags()
        {
            WorldState state = new WorldState();

            Assert.That(state.CurrentLocation, Is.Null);
            Assert.That(state.IsFlagSet("gate_open"), Is.False);
        }

        [Test]
        public void SetLocationRaisesEventWithPreviousAndNew()
        {
            WorldState state = new WorldState();
            LocationDefinition first = ScriptableObject.CreateInstance<LocationDefinition>();
            LocationDefinition second = ScriptableObject.CreateInstance<LocationDefinition>();
            List<(LocationDefinition Previous, LocationDefinition Current)> changes =
                new List<(LocationDefinition Previous, LocationDefinition Current)>();

            state.LocationChanged += (previous, current) => changes.Add((previous, current));
            state.SetLocation(first);
            state.SetLocation(second);

            Assert.That(state.CurrentLocation, Is.SameAs(second));
            Assert.That(changes.Count, Is.EqualTo(2));
            Assert.That(changes[0].Previous, Is.Null);
            Assert.That(changes[0].Current, Is.SameAs(first));
            Assert.That(changes[1].Previous, Is.SameAs(first));
            Assert.That(changes[1].Current, Is.SameAs(second));

            Object.DestroyImmediate(first);
            Object.DestroyImmediate(second);
        }

        [Test]
        public void SetLocationToSameLocationDoesNotRaiseEvent()
        {
            WorldState state = new WorldState();
            LocationDefinition location = ScriptableObject.CreateInstance<LocationDefinition>();
            int raised = 0;

            state.LocationChanged += (previous, current) => raised++;
            state.SetLocation(location);
            state.SetLocation(location);

            Assert.That(raised, Is.EqualTo(1));

            Object.DestroyImmediate(location);
        }

        [Test]
        public void FlagsRoundTrip()
        {
            WorldState state = new WorldState();

            state.SetFlag("tower_cleared", true);
            Assert.That(state.IsFlagSet("tower_cleared"), Is.True);

            state.SetFlag("tower_cleared", false);
            Assert.That(state.IsFlagSet("tower_cleared"), Is.False);
        }
    }
}
