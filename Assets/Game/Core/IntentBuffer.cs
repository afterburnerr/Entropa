namespace Entropa.Core
{
    public enum CharacterAction
    {
        Roll = 0,
        LightAttack = 1,
        HeavyAttack = 2
    }

    public sealed class IntentBuffer
    {
        private const float NotPressed = float.NegativeInfinity;

        private readonly float[] _lastPressedTimes = new float[(int)CharacterAction.HeavyAttack + 1];

        public IntentBuffer()
        {
            for (int i = 0; i < _lastPressedTimes.Length; i++)
            {
                _lastPressedTimes[i] = NotPressed;
            }
        }

        public void NotifyPressed(CharacterAction action, float now)
        {
            _lastPressedTimes[(int)action] = now;
        }

        public bool ConsumeIfPressedWithin(CharacterAction action, float now, float window)
        {
            int index = (int)action;
            if (now - _lastPressedTimes[index] > window)
            {
                return false;
            }

            _lastPressedTimes[index] = NotPressed;
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < _lastPressedTimes.Length; i++)
            {
                _lastPressedTimes[i] = NotPressed;
            }
        }
    }
}
