using Entropa.Cameras;
using Entropa.Character;
using Entropa.Core;
using Entropa.Gameplay;
using Entropa.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Entropa.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private LocationDefinition _startupLocation;
        [SerializeField] private string _startupPointId = "Default";

        protected override void Awake()
        {
            WorldRoot.RootScope = this;
            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (WorldRoot.RootScope == this)
            {
                WorldRoot.RootScope = null;
            }

            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraRig>();
            builder.RegisterComponentInHierarchy<CameraAnchor>();
            builder.RegisterComponentInHierarchy<CharacterTargeting>().As<ITargetProvider>();
            builder.RegisterComponentInHierarchy<PlayerBrain>();
            builder.RegisterComponentInHierarchy<PlayerAnchorBehaviour>().As<IPlayerAnchor>();

            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.Register<WorldState>(Lifetime.Singleton).As<IWorldState>();
            builder.Register<SceneTransitionPointLocator>(Lifetime.Singleton).As<ITransitionPointLocator>();
            builder.Register<TransitionService>(Lifetime.Singleton).As<ITransitionService>();

            builder.RegisterInstance(new WorldStartupSettings(_startupLocation, _startupPointId));
            builder.RegisterEntryPoint<WorldStartup>();
        }
    }
}
