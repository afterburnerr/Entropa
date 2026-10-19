namespace Entropa.Character
{
    public readonly struct ActionTiming
    {
        public readonly float Startup;
        public readonly float Active;
        public readonly float Recovery;
        public readonly float CancelStart;

        public ActionTiming(float startup, float active, float recovery, float cancelStart)
        {
            Startup = startup;
            Active = active;
            Recovery = recovery;
            CancelStart = cancelStart;
        }

        public float ActiveStart => Startup;

        public float ActiveEnd => Startup + Active;

        public float TotalDuration => Startup + Active + Recovery;
    }
}
