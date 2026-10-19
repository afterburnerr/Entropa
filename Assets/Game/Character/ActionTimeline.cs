namespace Entropa.Character
{
    public sealed class ActionTimeline
    {
        private readonly ActionTiming _timing;
        private float _elapsed;

        public ActionTimeline(ActionTiming timing)
        {
            _timing = timing;
        }

        public float Elapsed => _elapsed;

        public float NormalizedTime => _timing.TotalDuration > 0f ? _elapsed / _timing.TotalDuration : 1f;

        public bool IsFinished => _elapsed >= _timing.TotalDuration;

        public bool IsInStartup => _elapsed < _timing.ActiveStart;

        public bool IsActive => _elapsed >= _timing.ActiveStart && _elapsed < _timing.ActiveEnd;

        public bool IsInRecovery => _elapsed >= _timing.ActiveEnd;

        public bool IsCancelWindowOpen => _elapsed >= _timing.CancelStart;

        public void Advance(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            _elapsed += deltaTime;
        }
    }
}
