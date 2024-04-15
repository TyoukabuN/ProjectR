using System;
using System.Collections;
using System.Collections.Generic;
using PJR;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    public partial class PhysEntity: MonoBehaviour
    {
        [HideInInspector] public BoxCollider boxCollider;
        [HideInInspector] public SphereCollider sphereCollider;
        [HideInInspector] public CapsuleCollider capsuleCollider;
        [HideInInspector] public Collider attachedCollider;

        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionStay;
        public Action<Collision> onCollisionExit;
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        protected virtual void Init_Collection(GameObject avatar)
        {
            avatar = avatar != null ? avatar : this.avatar;
            attachedCollider = avatar.GetComponentInChildren<Collider>();

            var boxCollider = avatar.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null)
            {
                this.boxCollider = gameObject.CopyComponent(boxCollider) as BoxCollider;
                this.boxCollider.size = boxCollider.size;
                this.boxCollider.center = boxCollider.center;
                this.boxCollider.isTrigger = boxCollider.isTrigger;
                DestroyImmediate(boxCollider);
            }
            //
            var sphereCollider = avatar.GetComponentInChildren<SphereCollider>();
            if (sphereCollider != null)
            {
                this.sphereCollider = gameObject.CopyComponent(sphereCollider) as SphereCollider;
                this.sphereCollider.center = sphereCollider.center;
                this.sphereCollider.radius = sphereCollider.radius;
                this.sphereCollider.isTrigger = sphereCollider.isTrigger;
                DestroyImmediate(sphereCollider);
            }
            //
            var capsuleCollider = avatar.GetComponentInChildren<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                this.capsuleCollider = gameObject.CopyComponent(capsuleCollider) as CapsuleCollider;
                this.capsuleCollider.center = capsuleCollider.center;
                this.capsuleCollider.height = capsuleCollider.height;
                this.capsuleCollider.radius = capsuleCollider.radius;
                this.capsuleCollider.isTrigger = capsuleCollider.isTrigger;
                DestroyImmediate(capsuleCollider);
            }
        }

        protected virtual void Update_Collection()
        { 
        }

        public void InvokeCallback<TParam>(Action<TParam> callback, TParam param)
        {
            try
            {
                callback?.Invoke(param);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        protected virtual void OnCollisionEnter(Collision collision)
        {
            InvokeCallback(onCollisionEnter, collision);
        }
        protected virtual void OnCollisionStay(Collision collision)
        {
            InvokeCallback(onCollisionStay, collision);
        }
        protected virtual void OnCollisionExit(Collision collision)
        {
            InvokeCallback(onCollisionExit, collision);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            InvokeCallback(onTriggerEnter, other);
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            InvokeCallback(onTriggerStay, other);
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            InvokeCallback(onTriggerExit, other);
        }

        public bool IsColliderVaild(Collider2D collider)
        {
            if (collider == null || collider.gameObject == null)
                return false;
            return true;
        }
    }
}
