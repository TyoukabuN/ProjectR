using UnityEngine;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;
using System.Threading;

namespace PJR
{
    public partial class PhysEntity : MonoBehaviour , ICharacterController
    {
        [BoxGroup("运动控制")] public KinematicCharacterMotor motor;
        [BoxGroup("运动控制")] public Rigidbody rigidbody;
        [BoxGroup("运动控制")] public List<Collider> IgnoredColliders;

        public List<ICharacterController> ICharacterControllers;

        public Action<KCCContext> onUpdateVelocity;
        public Action<KCCContext> onUpdateRotation;

        protected virtual void Init_KCC(GameObject avatar)
        {
            rigidbody = gameObject.TryGetComponent<Rigidbody>();

            motor = gameObject.TryGetComponent<KinematicCharacterMotor>();
            motor.CharacterController = this;

            IgnoredColliders = new List<Collider>();
            ICharacterControllers = new List<ICharacterController>();
        }
        public void AfterCharacterUpdate(float deltaTime)
        {
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        //按调用顺序排好了
        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }
        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var context = new KCCContext
            {
                physEntity = this,
                inputRotation = currentRotation,
                motor = motor,
                deltaTime = deltaTime,
            };
            if (onUpdateRotation != null)
                onUpdateRotation.Invoke(context);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            var context = new KCCContext { 
                physEntity = this,
                inputVelocity = currentVelocity,
                motor = motor,
                deltaTime = deltaTime,
            };
            if (onUpdateVelocity != null)
                onUpdateVelocity.Invoke(context);

            currentVelocity = context.outputVelocity;
        }
    }
}