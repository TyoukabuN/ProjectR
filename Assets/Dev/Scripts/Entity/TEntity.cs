using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace PJR
{
    public partial class TEntity : MonoBehaviour
    {

        [HideInInspector] public PlatformEffector2D platformEffector2D;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public List<Animator> subAnimators = new List<Animator>();

        [HideInInspector] public Vector3 floorPoint = Vector3.zero;
        [HideInInspector] public int lastFloorObject = -1;

        private Transform m_ModelRoot = null;
        [HideInInspector] public Vector3 modelRoot_localPosition = Vector3.zero;
        [HideInInspector] public Vector3 modelRoot_localEulerAngles = Vector3.zero;
        [HideInInspector] public Vector3 modelRoot_localScale = Vector3.zero;

        public Transform ModelRoot {
            get {
                if (!m_ModelRoot)
                { 
                    m_ModelRoot = transform.Find(TEntityDefine.MODEL_ROOT_NAME);
                    if (m_ModelRoot)
                    {
                        modelRoot_localPosition = m_ModelRoot.localPosition;
                        modelRoot_localEulerAngles = m_ModelRoot.localEulerAngles;
                        modelRoot_localScale = m_ModelRoot.localScale;
                    }
                }
                return m_ModelRoot;
            }
        }

        public Action<float> onUpdateDistanceFromPlayer;
        public Action<int> onOutOfSight;
        public Action onAnimationClipEvent;

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
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            //
            m_onFixedUpdateBeginTrigger = true;

            Init_Collection();
            Init_Animation();
        }
        protected virtual void Start() {
        }

        protected virtual void Update() {
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


            //
            if (onUpdateDistanceFromPlayer != null)
            {
                var dif = Mathf.Abs(TinyGameManager.instance.GetPlayerPos().x - transform.position.x);
                try
                {
                    onUpdateDistanceFromPlayer.Invoke(dif);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            //
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
    }
}
