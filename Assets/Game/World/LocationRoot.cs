using UnityEngine;

namespace Entropa.World
{
    public sealed class LocationRoot : MonoBehaviour
    {
        [SerializeField] private LocationDefinition _location;

        public LocationDefinition Location => _location;
    }
}
