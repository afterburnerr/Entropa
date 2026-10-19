using System;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class CharacterStamina : MonoBehaviour
    {
        [SerializeField] private CharacterConfig _config;

        public StaminaModel Model { get; private set; }

        public event Action<float, float> Changed;

        public event Action Depleted;

        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError($"{name}: CharacterConfig is not assigned to {nameof(CharacterStamina)}.", this);
                enabled = false;
                return;
            }

            Model = new StaminaModel(_config.StaminaMax, _config.StaminaRegenPerSecond, _config.StaminaRegenDelay);
        }

        private void Update()
        {
            float before = Model.Current;
            Model.Tick(Time.deltaTime);

            if (!Mathf.Approximately(before, Model.Current))
            {
                Changed?.Invoke(Model.Current, Model.Max);
            }
        }

        public bool TrySpend(float cost)
        {
            float before = Model.Current;

            if (!Model.TrySpend(cost))
            {
                return false;
            }

            Changed?.Invoke(Model.Current, Model.Max);

            if (before > 0f && Model.IsDepleted)
            {
                Depleted?.Invoke();
            }

            return true;
        }

        public bool TryDrain(float amount)
        {
            float before = Model.Current;

            if (!Model.TryDrain(amount))
            {
                return false;
            }

            if (!Mathf.Approximately(before, Model.Current))
            {
                Changed?.Invoke(Model.Current, Model.Max);

                if (Model.IsDepleted)
                {
                    Depleted?.Invoke();
                }
            }

            return true;
        }
    }
}
