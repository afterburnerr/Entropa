using System;

namespace Entropa.World
{
    public interface IWorldState
    {
        LocationDefinition CurrentLocation { get; }

        event Action<LocationDefinition, LocationDefinition> LocationChanged;

        void SetLocation(LocationDefinition location);

        bool IsFlagSet(string flag);

        void SetFlag(string flag, bool value);
    }
}
