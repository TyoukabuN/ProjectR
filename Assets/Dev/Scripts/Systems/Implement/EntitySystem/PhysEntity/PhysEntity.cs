﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace PJR
{
    /// <summary>
    /// Physics Entity
    /// </summary>
    public partial class PhysEntity : MonoBehaviour
    {
        [HideInInspector] public PlatformEffector2D platformEffector2D;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        [HideInInspector] public Vector3 floorPoint = Vector3.zero;
        [HideInInspector] public int lastFloorObject = -1;

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
        public void InitModel()
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
