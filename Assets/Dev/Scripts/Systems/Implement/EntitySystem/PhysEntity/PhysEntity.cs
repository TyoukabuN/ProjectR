using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace PJR
{
    /// <summary>
    /// Physics Entity
    /// </summary>
    public partial class PhysEntity : MonoBehaviour
    {
        [ShowInInspector]
        [NonSerialized]
        [BoxGroup("逻辑引用"), ReadOnly] public LogicEntity logicEntity = null;

        [HideInInspector] public PlatformEffector2D platformEffector2D;

        [HideInInspector] public Vector3 floorPoint = Vector3.zero;
        [HideInInspector] public int lastFloorObject = -1;

        public int physEntityId = -1;

        public PhysEntityComponentRequire physRequire = PhysEntityComponentRequire.Default;

        public bool IsVaild()
        {
            //if (TinyGameManager.instance == null)
            //    return false;
            return true;
        }

        public virtual void OnDestroyBlock(Transform block) { }

        public virtual void OnStateChange(GameState state, GameState oldState) { }

        protected virtual void Awake()
        {
        }

        protected virtual void Start() {
        }

        protected virtual void Update() {
            if (!_avatarLoadDone)
                return;
            Update_Animation(Time.deltaTime);
            Update_Collection();
        }

        public virtual void OnDestroy() {
        }
        protected virtual void FixedUpdate()
        {
            if (!TinyGameManager.instance)
                return;

            if (TinyGameManager.instance.isPause)
            {
                Animator_SetEnabled(false);
            }
            else
            { 
                Animator_SetEnabled(true);
            }

            if (!TinyGameManager.instance.isRunning)
                return;
            //
            OnFixedUpdateBegin();
        }

        public virtual void Destroy()
        { 
            animancer.Stop();
        }

        [HideInInspector] public bool m_onFixedUpdateBeginTrigger = true;
        [HideInInspector] public Action onFixedUpdateBegin;

        public void OnFixedUpdateBegin()
        {
            if (!m_onFixedUpdateBeginTrigger)
                return;
            m_onFixedUpdateBeginTrigger = false;
            if (onFixedUpdateBegin != null)
            {
                try
                {
                    onFixedUpdateBegin.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }


        public void ChangeColliderMask(string[] layers)
        {
            if (platformEffector2D)
                platformEffector2D.colliderMask = LayerMask.GetMask(layers);
        }

        [HideInInspector]public float OutOfSightOffset = 0.1f;
        [HideInInspector]public bool isOutOfSight = true;

        public virtual void OnAnimationClipEvent()
        {
            if (onAnimationClipEvent != null)
            {
                try
                {
                    onAnimationClipEvent.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public Action onDrawGizmos;

        void OnDrawGizmos()
        {
            if(onDrawGizmos != null)
                onDrawGizmos.Invoke();
        }
    }
}
