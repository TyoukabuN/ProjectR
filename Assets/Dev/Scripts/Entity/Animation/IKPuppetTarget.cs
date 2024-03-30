using UnityEngine;

namespace TinyGame
{
    public sealed class IKPuppetTarget : MonoBehaviour
    {
        [SerializeField] private AvatarIKGoal _Type;
        [SerializeField, Range(0, 1)] private float _PositionWeight = 1;
        [SerializeField, Range(0, 1)] private float _RotationWeight = 0;

        public ValueChangeApproach valueChangeApproach = ValueChangeApproach.Tween;
        public AvatarIKGoal AvatarIKGoal => _Type;

        public float factor = 1f;
        private float _targetFactor = 0;
        public float tweenSpeed = 2.33f;

        public void UpdateAnimatorIK(Animator animator)
        {
            if(valueChangeApproach == ValueChangeApproach.Tween)
                factor = Mathf.MoveTowards(factor, _targetFactor, Time.deltaTime * tweenSpeed);

            animator.SetIKPositionWeight(_Type, _PositionWeight * factor);
            animator.SetIKRotationWeight(_Type, _RotationWeight * factor);

            animator.SetIKPosition(_Type, transform.position);
            animator.SetIKRotation(_Type, transform.rotation);
        }

        public void SetAnimatorIK(Animator animator,float _factor, ValueChangeApproach valueChangeApproach = ValueChangeApproach.Immediately)
        {
            if (valueChangeApproach == ValueChangeApproach.Immediately)
                this.factor = _factor;
            else
                _targetFactor = _factor;
            this.valueChangeApproach = valueChangeApproach;
        }
    }
}
