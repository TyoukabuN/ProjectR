using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using System;
using Sirenix.OdinInspector;

namespace PJR
{
    public partial class PhysEntity: MonoBehaviour, IAnimatorEventReceiver
    {
        [BoxGroup("动画相关")] public Animator mainAnimator;
        [BoxGroup("动画相关")] public List<Animator> subAnimators = new List<Animator>();
        [BoxGroup("动画相关")] public AnimancerComponent animancer;
        [BoxGroup("动画相关")] public List<AnimancerComponent> subAnimancers = new List<AnimancerComponent>();
        [BoxGroup("动画相关")] public AnimatorAgency animatorAgency;
        [BoxGroup("动画相关")] public AnimatiomClipTransitionSet animationClipTransitionSet;
        [BoxGroup("动画相关")] private List<AvatarMask> avatarMasks;
        public Action onAnimationClipEvent;


        [Serializable]
        public enum AnimationLayerType : int
        {
            Base = 0,
            Action = 1,
        }

        protected virtual void Init_Animation(GameObject avater)
        {
            avater = avater != null ? avater : this.avater;
            mainAnimator = avater.GetComponentInChildren<Animator>();
            animancer = avater.GetComponentInChildren<AnimancerComponent>();
            //
            animatorAgency = avater.GetComponentInChildren<AnimatorAgency>();
            animatorAgency.animatorEventReceiver = this;
        }
        protected void Setup_AvaterMask()
        {
            if (animancer == null)
                return;

            for (int layerIndex = 0; layerIndex < avatarMasks.Count; layerIndex++)
            {
                animancer.Layers[layerIndex].SetMask(avatarMasks[layerIndex]);
            }
        }
        public void Animator_SetEnabled(bool enabled)
        {
            if (mainAnimator)//|| !animator.HasState(layerIndex, stateId)
                mainAnimator.enabled = enabled;
            foreach (var animator in subAnimators)
                if (animator)
                    animator.enabled = enabled;
        }
        protected void CheckAniamtor()
        {
            if (TinyGameManager.instance.isPause)
                Animator_SetEnabled(false);
            else
                Animator_SetEnabled(true);
        }

        #region Animancer
        public bool TryGetClip(string clipName,out ClipTransition clip)
        {
            clip = null;
            if (animationClipTransitionSet == null || animationClipTransitionSet.clips == null)
                return false;
            return animationClipTransitionSet.clips.TryGetValue(clipName, out clip);
        }
        public AnimancerState Animancer_Play(string clipName)
        {
            return Animancer_Play(clipName,0);
        }
        public AnimancerState Animancer_Play(string clipName,int layerIndex)
        {
            if (animancer == null) return null;
            var layer = animancer.Layers[layerIndex];
            if (TryGetClip(clipName, out ClipTransition clip)) { 
                return layer.Play(clip);
            }
            return null;
        }
        public AnimancerState Animancer_Play(string clipName, float transition = 0.25f, FadeMode mode = default,int layerIndex = 0)
        {
            if (animancer == null) return null;
            var layer = animancer.Layers[layerIndex];
            if (TryGetClip(clipName, out ClipTransition clip))
            {
                return layer.Play(clip, transition, mode);
            }
            return null;
        }

        #endregion


        #region Animator
        public void Animator_Play(string stateName, int layerIndex = -1, float normalizedTime = 0)
        {
            Animator_Play(Animator.StringToHash(stateName), layerIndex, normalizedTime);
        }
        public void Animator_Play(int stateId, int layerIndex = -1, float normalizedTime = 0)
        {
            if (mainAnimator)//|| !animator.HasState(layerIndex, stateId)
                mainAnimator.Play(stateId, layerIndex, normalizedTime);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.Play(stateId, layerIndex, normalizedTime);

        }
        public void Animator_SetTrigger(string name)
        {
            if (mainAnimator)
                mainAnimator.SetTrigger(name);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetTrigger(name);
        }
        public void Animator_SetTrigger(int nameHash)
        {
            if (mainAnimator)
                mainAnimator.SetTrigger(nameHash);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetTrigger(nameHash);
        }
        public void Animator_SetInt(string name, int intValue)
        {
            if (mainAnimator)
                mainAnimator.SetInteger(name, intValue);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetInteger(name, intValue);
        }
        public void Animator_SetInt(int nameHash, int intValue)
        {
            if (mainAnimator)
                mainAnimator.SetInteger(nameHash, intValue);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetInteger(nameHash, intValue);
        }
        public void Animator_SetFloat(string name, float floatValue)
        {
            if (mainAnimator)
                mainAnimator.SetFloat(name, floatValue);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetFloat(name, floatValue);
        }
        public void Animator_SetFloat(int nameHash, float floatValue)
        {
            if (mainAnimator)
                mainAnimator.SetFloat(nameHash, floatValue);

            foreach (var animator in subAnimators)
                if (animator)
                    animator.SetFloat(nameHash, floatValue);
        }
        public void Animator_SetSpeed(float value = 1)
        {
            if (mainAnimator)
                mainAnimator.speed = value;

            foreach (var animator in subAnimators)
            {
                if (animator)
                {
                    animator.speed = value;
                }
            }
        }
        public void Animator_AddSubAnimator(Animator animator)
        {
            if (subAnimators.Contains(animator))
                return;
            subAnimators.Add(animator);
        }
        public virtual void OnAnimatorIK()
        {

        }
        public virtual void OnAnimatorMove()
        {

        }
        #endregion
    }


}
