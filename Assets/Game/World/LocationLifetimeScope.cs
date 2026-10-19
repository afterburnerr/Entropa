using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Entropa.World
{
    public sealed class LocationLifetimeScope : LifetimeScope
    {
        [SerializeField] private LocationDefinition _location;

        public LocationDefinition Location => _location;

        protected override LifetimeScope FindParent()
        {
            return WorldRoot.RootScope;
        }

        protected override void Awake()
        {
            if (WorldRoot.RootScope == null)
            {
                autoRun = false;
                Debug.LogError($"{name}: location scope requires the bootstrap scene to be loaded first.");
                return;
            }

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            if (_location != null)
            {
                builder.RegisterInstance(_location);
            }

            builder.RegisterComponentInHierarchy<LocationRoot>();

            if (FindInScene<TransitionPreloader>() != null)
            {
                builder.RegisterComponentInHierarchy<TransitionPreloader>();
            }

            if (FindInScene<TransitionExecutor>() != null)
            {
                builder.RegisterComponentInHierarchy<TransitionExecutor>();
            }
        }

        private T FindInScene<T>() where T : Component
        {
            Scene scene = gameObject.scene;
            if (!scene.IsValid())
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                T component = roots[i].GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }
    }
}
