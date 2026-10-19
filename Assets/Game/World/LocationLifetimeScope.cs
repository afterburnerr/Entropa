using UnityEngine;
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
            builder.RegisterComponentInHierarchy<TransitionPreloader>();
            builder.RegisterComponentInHierarchy<TransitionExecutor>();
        }
    }
}
