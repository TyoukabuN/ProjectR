using System;
using System.Collections;
using System.Collections.Generic;
using PJR;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    public partial class PhysEntity: MonoBehaviour
    {
        [HideInInspector] public BoxCollider boxCollider;
        [HideInInspector] public CapsuleCollider capsuleCollider;

        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionExit;
        public Action<Collision> onCollisionStay;
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerExit;

        //
        public virtual bool Grounded { get; }

        protected virtual void Init_Collection(GameObject avatar)
        {
            avatar = avatar != null ? avatar : this.avatar;
            boxCollider = avatar.GetComponent<BoxCollider>();
            boxCollider = boxCollider == null ? avatar.GetComponentInChildren<BoxCollider>() : boxCollider;
            if (boxCollider != null)
            {
                boxCollider = gameObject.CopyComponent(boxCollider) as BoxCollider;
            }
            //
            capsuleCollider = avatar.GetComponent<CapsuleCollider>();
            capsuleCollider = capsuleCollider == null ? avatar.GetComponentInChildren<CapsuleCollider>() : capsuleCollider;
            if (capsuleCollider != null)
            {
                capsuleCollider = gameObject.CopyComponent(capsuleCollider) as CapsuleCollider;
            }
        }

        protected virtual void Update_Collection()
        { 
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            if (onCollisionEnter != null)
            {
                try
                {
                    onCollisionEnter.Invoke(collision);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        public virtual void OnCollisionExit(Collision collision)
        {
            if (onCollisionExit != null)
            {
                try
                {
                    onCollisionExit.Invoke(collision);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (onCollisionStay != null)
            {
                try
                {
                    onCollisionStay.Invoke(collision);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        public virtual void OnTriggerEnter(Collider collider)
        {
            if (onTriggerEnter != null)
            {
                try
                {
                    onTriggerEnter.Invoke(collider);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        public virtual void OnTriggerExit(Collider collider)
        {
            if (onTriggerExit != null)
            {
                try
                {
                    onTriggerExit.Invoke(collider);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public bool IsColliderVaild(Collider2D collider)
        {
            if (collider == null || collider.gameObject == null)
                return false;
            return true;
        }

        public static float RaycastHit2DDistance = 100f;
        public Collider2D GetCurrentFloorCollider()
        {
            var res = GetCurrentFloorHit();
            if (!res)
                return null;
            return res.collider;
        }
        public RaycastHit2D GetCurrentFloorHit()
        {
            float dis = RaycastHit2DDistance;
            return new RaycastHit2D();
        }
    }
}
