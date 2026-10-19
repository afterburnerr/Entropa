using Entropa.Cameras;
using Entropa.Character;
using Entropa.Core;
using Entropa.Gameplay;
using VContainer;
using VContainer.Unity;

namespace Entropa.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraRig>();
            builder.RegisterComponentInHierarchy<CameraAnchor>();
            builder.RegisterComponentInHierarchy<CharacterTargeting>().As<ITargetProvider>();
            builder.RegisterComponentInHierarchy<PlayerBrain>();
        }
    }
}
