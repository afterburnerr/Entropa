using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entropa.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entropa.Tests.EditMode
{
    [TestFixture]
    public sealed class TransitionServiceTests
    {
        private sealed class FakeSceneLoader : ISceneLoader
        {
            public readonly List<LocationDefinition> Preloaded = new List<LocationDefinition>();
            public readonly List<LocationDefinition> Activated = new List<LocationDefinition>();
            public readonly List<LocationDefinition> Unloaded = new List<LocationDefinition>();

            public UniTask PreloadAsync(LocationDefinition location, CancellationToken cancellationToken = default)
            {
                Preloaded.Add(location);
                return UniTask.CompletedTask;
            }

            public UniTask ActivateAsync(LocationDefinition location, CancellationToken cancellationToken = default)
            {
                Activated.Add(location);
                return UniTask.CompletedTask;
            }

            public UniTask UnloadAsync(LocationDefinition location, CancellationToken cancellationToken = default)
            {
                Unloaded.Add(location);
                return UniTask.CompletedTask;
            }

            public bool IsActivated(LocationDefinition location)
            {
                return Activated.Contains(location);
            }

            public bool TryGetLoaded(LocationDefinition location, out Scene scene)
            {
                scene = default;
                return Activated.Contains(location);
            }
        }

        private sealed class FakeAnchor : IPlayerAnchor
        {
            public FakeAnchor(Transform root)
            {
                Root = root;
            }

            public Transform Root { get; }

            public void Teleport(Vector3 position, Quaternion rotation)
            {
                Root.SetPositionAndRotation(position, rotation);
            }
        }

        private sealed class FakePointLocator : ITransitionPointLocator
        {
            public TransitionPoint Point;
            public int Queries;

            public TransitionPoint Find(string sceneName, string pointId)
            {
                Queries++;
                return Point;
            }
        }

        private GameObject _anchorObject;
        private GameObject _pointObject;
        private TransitionPoint _point;
        private LocationDefinition _plaza;
        private LocationDefinition _tower;
        private FakeSceneLoader _loader;
        private FakePointLocator _locator;
        private WorldState _state;
        private TransitionService _service;

        [SetUp]
        public void SetUp()
        {
            _anchorObject = new GameObject("Anchor");
            _pointObject = new GameObject("Point");
            _point = _pointObject.AddComponent<TransitionPoint>();
            _pointObject.transform.SetPositionAndRotation(new Vector3(60f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));

            _plaza = ScriptableObject.CreateInstance<LocationDefinition>();
            _tower = ScriptableObject.CreateInstance<LocationDefinition>();
            _loader = new FakeSceneLoader();
            _locator = new FakePointLocator { Point = _point };
            _state = new WorldState();
            _service = new TransitionService(_loader, _state, _locator, new FakeAnchor(_anchorObject.transform));
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_anchorObject);
            UnityEngine.Object.DestroyImmediate(_pointObject);
            UnityEngine.Object.DestroyImmediate(_plaza);
            UnityEngine.Object.DestroyImmediate(_tower);
        }

        [Test]
        public void TransitionMovesAnchorToPointAndUpdatesState()
        {
            _service.TransitionAsync(_tower, "FromPlaza").GetAwaiter().GetResult();

            Assert.That(_state.CurrentLocation, Is.SameAs(_tower));
            Assert.That(_anchorObject.transform.position, Is.EqualTo(_pointObject.transform.position));
            Assert.That(_anchorObject.transform.rotation, Is.EqualTo(_pointObject.transform.rotation));
            Assert.That(_loader.Preloaded, Has.Member(_tower));
            Assert.That(_loader.Activated, Has.Member(_tower));
        }

        [Test]
        public void TransitionUnloadsPreviousLocation()
        {
            _state.SetLocation(_plaza);

            _service.TransitionAsync(_tower, "FromPlaza").GetAwaiter().GetResult();

            Assert.That(_loader.Unloaded, Has.Member(_plaza));
            Assert.That(_loader.Unloaded, Has.No.Member(_tower));
        }

        [Test]
        public void TransitionWithoutPointStillUpdatesState()
        {
            _locator.Point = null;

            _service.TransitionAsync(_tower, "Missing").GetAwaiter().GetResult();

            Assert.That(_state.CurrentLocation, Is.SameAs(_tower));
            Assert.That(_anchorObject.transform.position, Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void TransitionRaisesStartedAndCompletedEvents()
        {
            List<LocationDefinition> started = new List<LocationDefinition>();
            List<LocationDefinition> completed = new List<LocationDefinition>();
            _service.TransitionStarted += started.Add;
            _service.TransitionCompleted += completed.Add;

            _service.TransitionAsync(_tower, "FromPlaza").GetAwaiter().GetResult();

            Assert.That(started, Is.EqualTo(new[] { _tower }));
            Assert.That(completed, Is.EqualTo(new[] { _tower }));
            Assert.That(_service.IsTransitioning, Is.False);
        }
    }
}
