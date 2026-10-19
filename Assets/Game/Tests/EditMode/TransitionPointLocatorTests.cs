using Entropa.World;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entropa.Tests.EditMode
{
    [TestFixture]
    public sealed class TransitionPointLocatorTests
    {
        private GameObject _root;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("Location");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_root);
        }

        [Test]
        public void FindsPointInScene()
        {
            TransitionPoint child = CreatePoint("Door", new Vector3(4f, 0f, 2f));

            TransitionPoint found = new SceneTransitionPointLocator().Find(_root.scene, "Door");

            Assert.That(found, Is.SameAs(child));
        }

        [Test]
        public void ReturnsNullForUnknownPoint()
        {
            CreatePoint("Door", Vector3.zero);

            TransitionPoint found = new SceneTransitionPointLocator().Find(_root.scene, "Window");

            Assert.That(found, Is.Null);
        }

        [Test]
        public void ReturnsNullForUnknownScene()
        {
            Assert.That(new SceneTransitionPointLocator().Find(SceneManager.GetSceneByName("NoSuchScene"), "Door"), Is.Null);
        }

        private TransitionPoint CreatePoint(string pointId, Vector3 position)
        {
            GameObject pointObject = new GameObject($"Point_{pointId}");
            pointObject.transform.SetParent(_root.transform);
            pointObject.transform.position = position;
            TransitionPoint point = pointObject.AddComponent<TransitionPoint>();
            SerializedObject serialized = new SerializedObject(point);
            serialized.FindProperty("_pointId").stringValue = pointId;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return point;
        }
    }
}
