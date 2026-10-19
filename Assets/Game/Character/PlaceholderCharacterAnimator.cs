using UnityEngine;

namespace Entropa.Character
{
    public sealed class PlaceholderCharacterAnimator : MonoBehaviour, ICharacterAnimator
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private float _poseSmoothing = 14f;

        private Quaternion _restRotation;
        private Quaternion _targetRotation;
        private float _recoverAt = -1f;
        private bool _dead;

        private void Awake()
        {
            if (_visual == null)
            {
                _visual = transform;
            }

            _restRotation = _visual.localRotation;
            _targetRotation = _restRotation;
        }

        private void Update()
        {
            float rate = 1f - Mathf.Exp(-_poseSmoothing * Time.deltaTime);
            _visual.localRotation = Quaternion.Slerp(_visual.localRotation, _targetRotation, rate);

            if (_recoverAt >= 0f && Time.time >= _recoverAt)
            {
                _recoverAt = -1f;
                _targetRotation = _restRotation;
            }
        }

        public void SetLocomotion(float normalizedSpeed)
        {
        }

        public void PlayRoll(Vector3 worldDirection)
        {
            Pose(Quaternion.Euler(55f, 0f, 0f), 0.45f);
        }

        public void PlayAttack(AttackDefinition attack)
        {
            Pose(Quaternion.Euler(30f, 0f, 0f), 0.25f);
        }

        public void PlayHitReact()
        {
            Pose(Quaternion.Euler(-20f, 0f, 12f), 0.25f);
        }

        public void PlayDeath()
        {
            _dead = true;
            _recoverAt = -1f;
            _targetRotation = _restRotation * Quaternion.Euler(-90f, 0f, 0f);
        }

        private void Pose(Quaternion localDelta, float holdSeconds)
        {
            if (_dead)
            {
                return;
            }

            _targetRotation = _restRotation * localDelta;
            _recoverAt = Time.time + holdSeconds;
        }
    }
}
