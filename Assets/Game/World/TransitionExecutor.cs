using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entropa.Core;
using UnityEngine;
using VContainer;

namespace Entropa.World
{
    public sealed class TransitionExecutor : MonoBehaviour
    {
        [SerializeField] private LocationDefinition _target;
        [SerializeField] private string _pointId = "Default";

        private ITransitionService _transitionService;
        private bool _armed = true;

        [Inject]
        public void Construct(ITransitionService transitionService)
        {
            _transitionService = transitionService;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_armed || _transitionService == null || _target == null || other.gameObject.layer != GameLayers.Player)
            {
                return;
            }

            _armed = false;
            RunTransitionAsync().Forget();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != GameLayers.Player)
            {
                return;
            }

            _armed = true;
        }

        private async UniTaskVoid RunTransitionAsync()
        {
            try
            {
                await _transitionService.TransitionAsync(_target, _pointId, destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
