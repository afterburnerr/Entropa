using UnityEngine;

namespace Entropa.Character
{
    public sealed class StaminaModel
    {
        private readonly float _max;
        private readonly float _regenPerSecond;
        private readonly float _regenDelay;
        private float _timeSinceSpend;

        public StaminaModel(float max, float regenPerSecond, float regenDelay)
        {
            _max = max;
            _regenPerSecond = regenPerSecond;
            _regenDelay = regenDelay;
            Current = max;
        }

        public float Current { get; private set; }

        public float Max => _max;

        public float Normalized => _max > 0f ? Current / _max : 0f;

        public bool IsDepleted => Current <= 0f;

        public bool CanSpend(float cost)
        {
            return cost >= 0f && Current >= cost;
        }

        public bool TrySpend(float cost)
        {
            if (!CanSpend(cost))
            {
                return false;
            }

            Current -= cost;
            _timeSinceSpend = 0f;
            return true;
        }

        public bool TryDrain(float amount)
        {
            if (amount <= 0f)
            {
                return true;
            }

            if (Current <= 0f)
            {
                return false;
            }

            Current = Mathf.Max(0f, Current - amount);
            _timeSinceSpend = 0f;
            return Current > 0f;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            _timeSinceSpend += deltaTime;

            if (_timeSinceSpend < _regenDelay || Current >= _max)
            {
                return;
            }

            Current = Mathf.Min(_max, Current + _regenPerSecond * deltaTime);
        }
    }
}
