using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TG
{
    public sealed class RigConstraintHandle : MonoBehaviour
    {
        [SerializeField] private AvatarIKGoal _Type;
        /// <summary>
        /// record constraint default
        /// </summary>
        [SerializeField, Range(0, 1)] private float _PositionWeight = 1;
        [SerializeField, Range(0, 1)] private float _RotationWeight = 1;

        public ValueChangeApproach valueChangeApproach = ValueChangeApproach.Tween;
        public AvatarIKGoal AvatarIKGoal => _Type;

        public float factor = 1f;
        public float tweenSpeed = 2.33f;

        private float _targetFactor = 0;

        public TwoBoneIKConstraint _twoBoneIKconstraint;
        public MultiRotationConstraint _multiRotationConstraint;

        public void Awake()
        {
            _twoBoneIKconstraint = GetComponent<TwoBoneIKConstraint>();
            _multiRotationConstraint = GetComponent<MultiRotationConstraint>();

            if (_twoBoneIKconstraint) _PositionWeight = _twoBoneIKconstraint.weight;
            if (_multiRotationConstraint) _RotationWeight = _multiRotationConstraint.weight;
        }

        public void UpdateAnimatorIK()
        {
            if (valueChangeApproach == ValueChangeApproach.Tween)
            { 
                factor = Mathf.MoveTowards(factor, _targetFactor, Time.deltaTime * tweenSpeed);
                if (_twoBoneIKconstraint) _twoBoneIKconstraint.weight =  factor;
                if (_multiRotationConstraint) _multiRotationConstraint.weight =  factor;
            }
        }

        public void SetTargetPosition(Vector3 worldPosition)
        {
            if (_twoBoneIKconstraint != null && _twoBoneIKconstraint.data.target != null) 
                _twoBoneIKconstraint.data.target.transform.position = worldPosition;
        }

        public void SetAnimatorIK(float _factor, ValueChangeApproach valueChangeApproach = ValueChangeApproach.Immediately)
        {
            //Debug.Log("[SetAnimatorIK]  " + _factor.ToString("f2"));
            if (valueChangeApproach == ValueChangeApproach.Immediately)
            { 
                this.factor = _factor;
                if(_twoBoneIKconstraint) _twoBoneIKconstraint.weight =  factor;
                if (_multiRotationConstraint) _multiRotationConstraint.weight =  factor;
            }
            else
                _targetFactor = _factor;
            this.valueChangeApproach = valueChangeApproach;
        }
    }
}