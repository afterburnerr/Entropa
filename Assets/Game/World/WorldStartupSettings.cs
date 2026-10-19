namespace Entropa.World
{
    public sealed class WorldStartupSettings
    {
        public WorldStartupSettings(LocationDefinition startLocation, string startPointId)
        {
            StartLocation = startLocation;
            StartPointId = startPointId;
        }

        public LocationDefinition StartLocation { get; }

        public string StartPointId { get; }
    }
}
