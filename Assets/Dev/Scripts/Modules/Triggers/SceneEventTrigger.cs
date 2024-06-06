using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    [RequireComponent(typeof(BoxCollider))]
    public class SceneEventTrigger : MonoBehaviour
    {
        [LabelText("可触发次数")]
        [Tooltip("< 0 即不限次数")]
        public int CanTriggerTimes = 1;

        [DisableIf("@true")]
        [SerializeField]
        [ShowInInspector]
        private int triggerCount = 0;

        [HideInInspector]
        public BoxCollider boxCollider;
        [HideInInspector]
        public new Renderer renderer;

        public UnityEvent OnTriggerEnterEvent = new UnityEvent();
        public UnityEvent OnTriggerEnterStay = new UnityEvent();
        public UnityEvent OnTriggerEnterExit = new UnityEvent();
        private void Awake()
        {
            triggerCount = 0;

            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }
        public bool IsValid()
        { 
            if(CanTriggerTimes < 0)
                return true;
            return triggerCount < CanTriggerTimes;
        }
        protected bool DoTrigger()
        {
            if (!IsValid())
                return false;
            triggerCount ++;
            return true;
        }
        public void Reset()
        {
            triggerCount = 0;
        }
        public bool IsColliderValid(Collider other,out PhysEntity physEntity)
        {
            physEntity = EntitySystem.GetPhysEntityByGobjInstanceId(other.gameObject.GetInstanceID());
            if (physEntity == null)
                return false;
            if (physEntity.logicEntity.entityContext.entityType != EntityDefine.LogicEntityType.Player)
                return false;
            return true;
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!IsValid()) return;
            if (!IsColliderValid(other, out var physEntity))
                return;
            if (!DoTrigger())
                return;
            try
            {
                OnTriggerEnterEvent.Invoke();
            }
            catch (Exception e)
            { 
                LogSystem.LogError(e);
            }
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!IsValid()) return;
            if (!IsColliderValid(other, out var physEntity))
                return;
            if (!DoTrigger())
                return;
            try
            {
                OnTriggerEnterStay.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.LogError(e);
            }
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!IsValid()) return;
            if (!IsColliderValid(other, out var physEntity))
                return;
            if (!DoTrigger())
                return;
            try
            {
                OnTriggerEnterExit.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.LogError(e);
            }
        }

        protected void OnDrawGizmos()
        {
            if (boxCollider == null)
                return;
            Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(SceneEventTrigger))]
    public class SceneEventTriggerEditor : Editor
    {
        private BoxCollider boxCollider;
        public void OnSceneGUI()
        {
            if (target == null)
                return;
            SceneEventTrigger inst = (SceneEventTrigger)target;
            boxCollider = inst.GetComponent<BoxCollider>();
            Handles.DrawWireCube(inst.transform.position + boxCollider.center, boxCollider.size);
        }
    }
#endif
}
