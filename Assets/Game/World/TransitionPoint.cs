using UnityEngine;

namespace Entropa.World
{
    public sealed class TransitionPoint : MonoBehaviour
    {
        [SerializeField] private string _pointId = "Default";

        public string PointId => _pointId;
    }
}
