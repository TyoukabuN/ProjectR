using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    public class CollisionAgency : MonoBehaviour
    {
        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionStay;
        public Action<Collision> onCollisionExit;
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

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
    }
}