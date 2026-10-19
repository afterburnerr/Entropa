using UnityEngine;

namespace Entropa.World
{
    [CreateAssetMenu(menuName = "Entropa/World/Location Definition", fileName = "Location_")]
    public sealed class LocationDefinition : ScriptableObject
    {
        [SerializeField] private string _locationId = "location";
        [SerializeField] private string _displayName = "Location";
        [SerializeField] private string _sceneName = "Location";
        [SerializeField] private string _musicId = "";

        public string LocationId => _locationId;

        public string DisplayName => _displayName;

        public string SceneName => _sceneName;

        public string MusicId => _musicId;
    }
}
