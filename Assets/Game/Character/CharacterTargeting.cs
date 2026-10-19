using System.Collections.Generic;
using Entropa.Combat;
using Entropa.Core;
using UnityEngine;

namespace Entropa.Character
{
    public sealed class CharacterTargeting : MonoBehaviour, ITargetProvider
    {
        [SerializeField] private CharacterConfig _config;
        [SerializeField] private Transform _viewSource;

        private readonly List<ITargetable> _candidates = new List<ITargetable>();
        private ITargetable _current;

        public bool HasTarget => IsValidTarget(_current);

        public Transform TargetTransform => IsValidTarget(_current) ? _current.TargetTransform : null;

        public Vector3 TargetPoint => IsValidTarget(_current) ? _current.TargetPoint : transform.position;

        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError($"{name}: CharacterConfig is not assigned to {nameof(CharacterTargeting)}.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_current == null)
            {
                return;
            }

            if (!IsValidTarget(_current))
            {
                _current = null;
                return;
            }

            Vector3 offset = _current.TargetPoint - transform.position;
            if (offset.sqrMagnitude > _config.LockOnReleaseRadius * _config.LockOnReleaseRadius)
            {
                _current = null;
            }
        }

        public void ToggleTarget()
        {
            if (HasTarget)
            {
                ClearTarget();
                return;
            }

            AcquireTarget();
        }

        public void ClearTarget()
        {
            _current = null;
        }

        public bool SwitchTarget(int direction)
        {
            if (direction == 0 || !HasTarget)
            {
                return false;
            }

            CollectCandidates();

            if (_candidates.Count <= 1)
            {
                return false;
            }

            Vector3 forward = GetViewForward();
            float currentAngle = SignedAngleTo(forward, _current.TargetPoint);
            ITargetable best = null;
            float bestDelta = float.MaxValue;

            for (int i = 0; i < _candidates.Count; i++)
            {
                ITargetable candidate = _candidates[i];

                if (ReferenceEquals(candidate, _current))
                {
                    continue;
                }

                float angle = SignedAngleTo(forward, candidate.TargetPoint);
                float delta = direction > 0
                    ? NormalizeAngle(angle - currentAngle)
                    : NormalizeAngle(currentAngle - angle);

                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    best = candidate;
                }
            }

            if (best == null)
            {
                return false;
            }

            _current = best;
            return true;
        }

        private void AcquireTarget()
        {
            CollectCandidates();
            Vector3 forward = GetViewForward();
            float halfAngle = _config.LockOnConeHalfAngle;
            ITargetable bestInCone = null;
            float bestAngle = float.MaxValue;
            ITargetable nearest = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < _candidates.Count; i++)
            {
                ITargetable candidate = _candidates[i];
                Vector3 offset = candidate.TargetPoint - transform.position;
                float distance = offset.magnitude;
                Vector3 planar = new Vector3(offset.x, 0f, offset.z);
                float angle = planar.sqrMagnitude > 0.0001f ? Vector3.Angle(forward, planar) : 0f;

                if (angle <= halfAngle && angle < bestAngle)
                {
                    bestAngle = angle;
                    bestInCone = candidate;
                }

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = candidate;
                }
            }

            _current = bestInCone != null ? bestInCone : nearest;
        }

        private void CollectCandidates()
        {
            _candidates.Clear();
            IReadOnlyList<ITargetable> targets = TargetRegistry.Active;
            float radiusSquared = _config.LockOnRadius * _config.LockOnRadius;

            for (int i = 0; i < targets.Count; i++)
            {
                ITargetable candidate = targets[i];

                if (!IsValidTarget(candidate) || candidate.TargetTransform == transform)
                {
                    continue;
                }

                Vector3 offset = candidate.TargetPoint - transform.position;

                if (offset.sqrMagnitude > radiusSquared)
                {
                    continue;
                }

                _candidates.Add(candidate);
            }
        }

        private Vector3 GetViewForward()
        {
            Vector3 forward = _viewSource != null ? _viewSource.forward : transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
            {
                return Vector3.forward;
            }

            return forward.normalized;
        }

        private float SignedAngleTo(Vector3 forward, Vector3 worldPoint)
        {
            Vector3 offset = worldPoint - transform.position;
            Vector3 planar = new Vector3(offset.x, 0f, offset.z);

            if (planar.sqrMagnitude < 0.0001f)
            {
                return 0f;
            }

            return Vector3.SignedAngle(forward, planar, Vector3.up);
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            return angle < 0f ? angle + 360f : angle;
        }

        private static bool IsValidTarget(ITargetable candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            if (candidate is Object unityObject && unityObject == null)
            {
                return false;
            }

            return candidate.IsTargetable;
        }
    }
}
