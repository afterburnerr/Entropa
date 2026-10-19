using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entropa.World
{
    public sealed class WorldState : IWorldState
    {
        private readonly HashSet<string> _flags = new HashSet<string>();

        public LocationDefinition CurrentLocation { get; private set; }

        public event Action<LocationDefinition, LocationDefinition> LocationChanged;

        public void SetLocation(LocationDefinition location)
        {
            if (CurrentLocation == location)
            {
                return;
            }

            LocationDefinition previous = CurrentLocation;
            CurrentLocation = location;
            LocationChanged?.Invoke(previous, location);
        }

        public bool IsFlagSet(string flag)
        {
            return !string.IsNullOrEmpty(flag) && _flags.Contains(flag);
        }

        public void SetFlag(string flag, bool value)
        {
            if (string.IsNullOrEmpty(flag))
            {
                Debug.LogWarning("World flag name is empty.");
                return;
            }

            if (value)
            {
                _flags.Add(flag);
            }
            else
            {
                _flags.Remove(flag);
            }
        }
    }
}
