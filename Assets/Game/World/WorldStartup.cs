using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Entropa.World
{
    public sealed class WorldStartup : IStartable
    {
        private readonly WorldStartupSettings _settings;
        private readonly ITransitionService _transitionService;

        public WorldStartup(WorldStartupSettings settings, ITransitionService transitionService)
        {
            _settings = settings;
            _transitionService = transitionService;
        }

        public void Start()
        {
            if (_settings.StartLocation == null)
            {
                return;
            }

            RunAsync().Forget();
        }

        private async UniTaskVoid RunAsync()
        {
            try
            {
                await _transitionService.TransitionAsync(_settings.StartLocation, _settings.StartPointId);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
