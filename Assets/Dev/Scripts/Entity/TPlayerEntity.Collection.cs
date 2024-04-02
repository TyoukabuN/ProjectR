using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define = PJR.TEntityDefine;

namespace PJR
{
    public partial class TPlayerEntity : StateMachineEntity, INumericalControl, IActionControl
    {
        [Range(0, 90)] public float maxGroundAngle = 25f;
        private float minGroundDotProduct
        {
            get
            {
                return Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            }
        }

        private List<int> StayingFloors = new List<int>();
        [HideInInspector] public int m_lastStayingFloorID = -1;
        [HideInInspector] public int obstructColliderID = 0;

        Vector3 contactNormal;

        bool desiredJump;

        int groundContactCount;

        protected override void Init_Collection()
        {
            base.Init_Collection();

            _rigidbody = GetComponent<Rigidbody>();

            onCollisionEnter = (Collision collision) =>
            {
                if (collision == null || collision.gameObject == null)
                    return;
                Collision_Evaluate(collision);
                if (groundContactCount > 0)
                {
                    Collision_OnGrounded();
                    SetJumpCount(0);
                }
            };

            onCollisionStay = (Collision collision) => Collision_Evaluate(collision);

            onCollisionExit = (Collision collision) =>
            {
                if (collision == null || collision.gameObject == null)
                    return;
                var instanceId = collision.gameObject.GetInstanceID();
                if (Collision_ExistStayingFloor(instanceId))
                {
                    Collision_RemoveStayingFloor(instanceId);
                    if (!Grounded)
                    {
                        obstructColliderID = 0;

                        if (jumpCounter <= 0)
                        {
                            //this.ExtraActionMapAdd((int)TPlayerActionType.KeepOnFloor, TinyGameManager.instance.fixedDeltaTime * 5f, false);
                            //this.ExtraVelocityMapAdd(Define.KD_TINYGRAVITY, GetGravitySign() * Physics.gravity * 0.1f, TinyGameManager.instance.fixedDeltaTime * 5f);
                        }

                        //var tactionCur = this.GetExtraActionMap(TPlayerActionType.KeepOnFloorPoint);
                        //var taction = this.ExtraActionMapAdd((int)TPlayerActionType.KeepOnFloorPoint, -1);
                        //taction.tActionEvent = new TActionEvent()
                        //{
                        //    vector3Value = tactionCur != null ? tactionCur.tActionEvent.vector3Value : floorPoint,
                        //};
                    }
                }
            };

            onTriggerEnter = (Collider collider) =>
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    if (!collider.isTrigger)
                    {
                        Collision_AddStayingFloor(collider.gameObject);
                    }
                }
            };
            SetAsDefaultColliderMask();
        }

        void Collision_OnGrounded()
        {
            //Animation_OnJumpLand();
            //states[(int)currentEState]?.OnGrounded();
            //State_Change(IsMoving() ? EPlayerState.Walk : EPlayerState.Stand);
        }

        void Collision_Evaluate(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                if (normal.y >= minGroundDotProduct)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                    Collision_AddStayingFloor(collision);
                }
            }
        }
        void Collision_ClearState()
        {
            groundContactCount = 0;
            contactNormal = Vector3.zero;
        }

        void Collision_UpdateState()
        {
            if (Grounded)
            {
                SetJumpCount(0);
                if (groundContactCount > 1)
                {
                    contactNormal.Normalize();
                }
            }
            else
            {
                contactNormal = Vector3.up;
            }
        }
        protected void Collision_SetRigidbodySimulated(bool simulated)
        {
            _rigidbody.isKinematic = simulated;
        }

        public override bool Grounded
        {
            get
            {
                return groundContactCount > 0 || (StayingFloors != null && StayingFloors.Count > 0) ;
            }
        }

        public Action<bool> onGroundedChange;
        public void OnGroundedChange()
        {
            if (onGroundedChange != null)
            {
                try
                {
                    onGroundedChange.Invoke(Grounded);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        public void Collision_AddStayingFloor(Collision collision)
        {
            if (collision == null || collision.gameObject == null) return;
            Collision_AddStayingFloor(collision.gameObject.GetInstanceID());
        }
        public void Collision_AddStayingFloor(GameObject gobj)
        {
            if (gobj == null) return;
            Collision_AddStayingFloor(gobj.GetInstanceID());
        }
        public void Collision_AddStayingFloor(int instanceId)
        {
            if (StayingFloors == null)
                StayingFloors = new List<int>();

            if (!StayingFloors.Contains(instanceId))
            {
                StayingFloors.Add(instanceId);
                m_lastStayingFloorID = instanceId;
            }

            OnGroundedChange();
        }

        public void Collision_RemoveStayingFloor(int instanceId)
        {
            if (StayingFloors == null)
                StayingFloors = new List<int>();

            if (StayingFloors.Contains(instanceId))
            {
                StayingFloors.Remove(instanceId);
            }

            OnGroundedChange();
        }

        public bool Collision_ExistStayingFloor(int instanceId)
        {
            if (StayingFloors == null)
                StayingFloors = new List<int>();

            return StayingFloors.Contains(instanceId);
        }
        public void Collision_ClearStayingFloor()
        {
            if (StayingFloors == null)
                StayingFloors = new List<int>();

            StayingFloors.Clear();
        }

        public bool Collision_CheckStayingFloor(ref Vector3 _floorUp)
        {
            float dis = 100.0f;
            RaycastHit hit;
            var res = Physics.Raycast(transform.position + Vector3.up * dis, Vector3.down, out hit, dis * 1.1f);
            if (!res)
            {
                return false;
            }
            _floorUp = hit.normal;
            lastFloorObject = hit.collider.gameObject.GetInstanceID();
            return true;
        }
        public bool Collision_CheckStayingFloorAndDistance(out float distance)
        {
            //float dis = 100.0f;
            //distance = -9999;
            //var res = Physics2D.Raycast(transform.position + Vector3.up * dis, Vector3.down, dis * 1.1f, TBlockUtility.FloorLayer);
            //if (!res)
            //    return false;
            //floorUp = res.normal;
            //distance = transform.position.y - res.point.y;
            distance = 1;
            return true;
        }
    }
}
