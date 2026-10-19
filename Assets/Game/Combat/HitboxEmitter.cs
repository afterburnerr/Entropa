using System.Collections.Generic;
using Entropa.Core;
using UnityEngine;

namespace Entropa.Combat
{
    public sealed class HitboxEmitter : MonoBehaviour
    {
        [SerializeField] private Transform _origin;

        private readonly Collider[] _overlapResults = new Collider[16];
        private readonly HashSet<Hurtbox> _alreadyHit = new HashSet<Hurtbox>();

        private DamageInfo _damageTemplate;
        private Vector3 _localOffset;
        private float _radius;
        private bool _windowOpen;
        private GameObject _owner;

        private void Awake()
        {
            _owner = transform.root.gameObject;

            if (_origin == null)
            {
                _origin = transform;
            }
        }

        public void BeginWindow(float damage, float poiseDamage, Vector3 localOffset, float radius)
        {
            _damageTemplate = new DamageInfo
            {
                Amount = damage,
                PoiseDamage = poiseDamage,
                Source = _owner
            };
            _localOffset = localOffset;
            _radius = radius;
            _alreadyHit.Clear();
            _windowOpen = true;
        }

        public void EndWindow()
        {
            _windowOpen = false;
            _alreadyHit.Clear();
        }

        private void FixedUpdate()
        {
            if (!_windowOpen)
            {
                return;
            }

            Vector3 point = _origin.TransformPoint(_localOffset);
            int count = Physics.OverlapSphereNonAlloc(
                point,
                _radius,
                _overlapResults,
                GameLayers.CharacterMask,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                Collider collider = _overlapResults[i];
                Hurtbox hurtbox = collider.GetComponentInParent<Hurtbox>();

                if (hurtbox == null || hurtbox.transform.root.gameObject == _owner)
                {
                    continue;
                }

                if (!_alreadyHit.Add(hurtbox))
                {
                    continue;
                }

                DamageInfo damage = _damageTemplate;
                damage.HitPoint = collider.ClosestPoint(point);
                damage.Direction = collider.transform.position - _owner.transform.position;
                hurtbox.TryReceiveDamage(in damage);
            }
        }
    }
}
