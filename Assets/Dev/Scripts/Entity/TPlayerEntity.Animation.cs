using System;
using UnityEngine;
using Animancer;
using UnityEngine.Animations.Rigging;
using static TinyGame.TEntity;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TinyGame
{
    [DefaultExecutionOrder(1000)]
    [Serializable]
    public partial class TPlayerEntity : StateMachineEntity, INumericalControl, IActionControl
    {
        public const string CNAME_IDLE = "Idle";

        public const string CNAME_JUMP_START = "Jump_Start";
        public const string CNAME_JUMP_KEEP = "Jump_Keep";
        public const string CNAME_JUMP_LAND_W = "Jump_Land_Wait";
        public const string CNAME_JUMP_LAND_M = "Jump_Land_Move";

        struct DirectionNameSet
        {
            public string F, FL, FR, B, BL, BR;
        }

        private static DirectionNameSet Walk = new DirectionNameSet()
        {
            F = "Walk_Front",
            FL = "Walk_Front_L",
            FR = "Walk_Front_R",
            B = "Walk_Back",
            BL = "Walk_Back_L",
            BR = "Walk_Back_R"
        };
        private static DirectionNameSet Dash = new DirectionNameSet()
        {
            F = "Dash_Front",
            FL = "Dash_Front_L",
            FR = "Dash_Front_R",
            B = "Dash_Back",
            BL = "Dash_Back_L",
            BR = "Dash_Back_R"
        };

        [SerializeField] protected bool _keepChildrenConnected = true;
        [SerializeField] protected AnimanerUpdateAproach _animanerUpdateAproach = AnimanerUpdateAproach.Manually;
        protected RigBuilder _rigBuilder;
        protected override void Init_Animation()
        {
            base.Init_Animation();

            _rigBuilder = GetComponentInChildren<RigBuilder>();

            if (animancer != null)
            {
                //IK Target
                //https://kybernetik.com.au/animancer/docs/examples/integration/animation-rigging/#ik-targets
                if (_rigBuilder != null)
                {
                    animancer.InitializePlayable(_rigBuilder.graph);
                    animancer.Playable.KeepChildrenConnected = _keepChildrenConnected;
                    animancer.Layers[0].ApplyAnimatorIK = true;
                }
                //For Rebind iusse
                //https://kybernetik.com.au/animancer/docs/examples/integration/animation-rigging/#limitations
                if (animationClipTransitionSet)
                {
                    foreach (var pair in animationClipTransitionSet.clips) {
                        animancer.Layers[0].GetOrCreateState(pair.Value);
                    }
                }

                if (_animanerUpdateAproach == AnimanerUpdateAproach.Manually)
                    animancer.Playable.PauseGraph();

                Setup_AvaterMask();

                animancer.Animator.applyRootMotion = false;
            }


            Animation_InitIK();
        }
        private bool CanPlayMovementAnima()
        {
            return Animation_CanExitJump() && Animation_CanExitAttack();
        }
        protected void Update_Animation()
        {
            //Animation_UpdateMovement();
            Animation_UpdateIK_RigConstraint();
            Animation_UpdateAnimancer();
        }

        protected void FixedUpdate_Animation()
        {
            if (!Animation_Attacking())
            {
                if (IKApproach.Equals(IKApproachType.OldSchool))
                {
                    if (upperarm_l) Animatiom_HandWallInteractiveIK(AvatarIKGoal.LeftHand, upperarm_l);
                    if (upperarm_r) Animatiom_HandWallInteractiveIK(AvatarIKGoal.RightHand, upperarm_r);
                }
                else
                {
                    if (upperarm_l) Animatiom_HandWallInteractive_TwoBoneIKConstraint(AvatarIKGoal.LeftHand, upperarm_l);
                    if (upperarm_r) Animatiom_HandWallInteractive_TwoBoneIKConstraint(AvatarIKGoal.RightHand, upperarm_r);
                }
            }
            else
            {
                if (IKApproach.Equals(IKApproachType.OldSchool))
                    Animatiom_SetIKActive(false);
                else
                    Animatiom_SetIKActive_RigConstraint(false);
            }
        }

        /// <summary>
        /// play movement animation according to user input
        /// </summary>
        protected void Animation_UpdateMovement()
        {
            canPlayMovementAnima = false;

            if (Grounded && CanPlayMovementAnima())
            {
                Animation_ClearJump();
                Animation_ClearAttack();
                //
                DirectionNameSet moveSet = Walk;
                if (IsRunning())
                    moveSet = Dash;

                if (inputAxi.magnitude > 0)
                {
                    if (Mathf.Abs(inputAxi.x) <= 0.001f)
                    {
                        Animancer_Play(inputAxi.y > 0 ? moveSet.F : moveSet.B);
                    }
                    else
                    {
                        if (inputAxi.y > 0)
                            Animancer_Play(inputAxi.x > 0 ? moveSet.FR : moveSet.FL);
                        else
                            Animancer_Play(inputAxi.x > 0 ? moveSet.BR : moveSet.BL);
                    }
                }
                else
                {
                    Animancer_Play(CNAME_IDLE);
                }
                canPlayMovementAnima = true;
            }
        }
        private bool canPlayMovementAnima = false;

        protected void Animation_UpdateAnimancer()
        {

            if (_animanerUpdateAproach == AnimanerUpdateAproach.Manually)
            {
                animancer.Evaluate(Time.deltaTime);
            }
            else if (animancer != null && animancer.IsPlayableInitialized && !animancer.Playable.IsPlaying())
            {
                animancer.Playable.UnpauseGraph();
            }
        }

        #region Jump

        private AnimancerState jumpAnimaState;
        protected bool Animation_CanExitJump()
        {
            if (jumpAnimaState == null)
                return true;
            return jumpAnimaState.NormalizedTime > 0.75f;
        }

        private void Animation_ClearJump()
        {
            if (jumpAnimaState != null)
                jumpAnimaState = null;
        }
        #endregion

        #region Attack Combo

        private string[] attackAnimaNameSet_Hand = new string[] { "Attack_Hand_1", "Attack_Hand_2", "Attack_Hand_3" };
        private float[] exitTimeSet_Hand = new float[] { 0.30f, 0.30f, 0.40f };
        private AttackState attackState;
        [SerializeField] public AnimationLayerType animationLayerType = AnimationLayerType.Base;
        private void Animation_Attack()
        {
            //start
            if (attackState == null || attackState.Done())
            {
                attackState = attackState == null ? new AttackState() : attackState;
                attackState.Init(this, attackAnimaNameSet_Hand, exitTimeSet_Hand, Animation_OnComboEnd);
                attackState.layerType = animationLayerType;
            }
            //combo
            attackState.Next();
        }
        private void Animation_OnComboEnd()
        {
            if (attackState != null) {
                if (attackState.IsLayer(animationLayerType))
                { 
                    animancer.Layers[(int)animationLayerType].Weight = 0;
                    animancer.Layers[(int)animationLayerType].Stop();
                }
                attackState.Clear();
                attackState = null;
            }
        }
        private bool Animation_CanExitAttack()
        {
            return attackState == null || attackState.CanExit() || !attackState.IsLayer(AnimationLayerType.Base);
        }
        private void Animation_ClearAttack()
        {
            if (attackState != null && attackState.IsLayer(AnimationLayerType.Base))
            { 
                attackState.Clear();
                attackState = null;
            }
        }
        private bool Animation_Attacking()
        {
            return !Animation_CanExitAttack();
        }
        public override void OnAnimatorMove()
        {
            if (mainAnimator.applyRootMotion)
            {
                _rigidbody.MovePosition(_rigidbody.position + mainAnimator.deltaPosition);
                _rigidbody.MoveRotation(_rigidbody.rotation * mainAnimator.deltaRotation);
            }
        }

        #endregion

        #region IK

        public enum IKApproachType
        {
            /// <summary>
            /// Animator IK for Humanoid Rigs 
            /// </summary>
            OldSchool,
            /// <summary>
            /// Using AnimatiomRigging Constraint to implement IK
            /// </summary>
            AnimationRigging
        }
        [SerializeField] private IKApproachType IKApproach = IKApproachType.AnimationRigging;
        [SerializeField] private IKPuppetTarget[] _IKTargets;
        [SerializeField] private RigConstraintHandle[] _RigConstraintHandles;

        public string path_upperarm_l = "root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l";
        public string path_upperarm_r = "root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r";
        public Transform upperarm_l;
        public Transform upperarm_r;

        private void Animation_InitIK()
        {
            animancer.Layers[0].ApplyAnimatorIK = true;
            Animatiom_SetIKActive(false);
            Animatiom_SetIKActive_RigConstraint(false);
            //
            var model = mainAnimator.transform;
            upperarm_l = model.transform.Find(path_upperarm_l);
            upperarm_r = model.transform.Find(path_upperarm_r);
        }
        public float[] rayDistance = new float[3] { 0.72f, 0.65f, 0.5f };
        public float IKInteractivePointOffset = -0.1f;

        public Vector3[] rayDirections = new Vector3[3];
        private void Animation_OnDrawGizmos()
        {

        }
        /// <summary>
        /// 手部与墙体交互的IK逻辑
        /// </summary>
        /// <param name="avatarIKGoal"></param>
        /// <param name="shoulder"></param>
        public void Animatiom_HandWallInteractiveIK(AvatarIKGoal avatarIKGoal, Transform shoulder)
        {
            if (shoulder == null)
                return;

            bool found = false;
            Vector3 interactivePoint = default;
            Vector3 side = transform.right;
            if (avatarIKGoal == AvatarIKGoal.LeftHand)
                side = -side;

            RaycastHit raycastHit;

            rayDirections[0] = transform.forward;
            rayDirections[1] = (forward + side).normalized;
            rayDirections[2] = side;

            for (int i = 0; i < rayDirections.Length; i++)
            {
                var direction = rayDirections[i];
                if (Physics.Raycast(shoulder.position, direction, out raycastHit, rayDistance[i], 1 << LayerMask.NameToLayer("Wall")))
                {
                    Vector3 normal = -raycastHit.normal;
                    Vector3 dir = (raycastHit.point - shoulder.position);
                    dir = normal * (Vector3.Dot(dir, normal) + IKInteractivePointOffset);
                    interactivePoint = shoulder.position + dir;
                    found = true;
                    //if (isDebug) Gizmos.color = Color.yellow;
                    //if (isDebug) Handles.DrawLine(shoulder.position, shoulder.position + dir);
                    break;
                }
                else
                {
                    if (isDebug)
                    {
                        //Gizmos.color = Color.red;
                        //Handles.DrawLine(shoulder.position, shoulder.position + direction * rayDistance[i]);
                    }
                }
            }

            if (found)
            {
                Animatiom_IKSwitch(avatarIKGoal, true, interactivePoint, ValueChangeApproach.Tween);
            }
            else
            {
                Animatiom_IKSwitch(avatarIKGoal, false);
            }
        }


        /// <summary>
        /// 手部与墙体交互的IK逻辑,使用AnimationRigging的TwoBoneIKConstrain
        /// </summary>
        /// <param name="avatarIKGoal"></param>
        /// <param name="shoulder"></param>
        public void Animatiom_HandWallInteractive_TwoBoneIKConstraint(AvatarIKGoal avatarIKGoal, Transform shoulder)
        {
            if (shoulder == null)
                return;

            bool found = false;
            Vector3 interactivePoint = default;
            Vector3 side = transform.right;
            if (avatarIKGoal == AvatarIKGoal.LeftHand)
                side = -side;

            RaycastHit raycastHit;

            rayDirections[0] = transform.forward;
            rayDirections[1] = (forward + side).normalized;
            rayDirections[2] = side;

            for (int i = 0; i < rayDirections.Length; i++)
            {
                var direction = rayDirections[i];
                if (Physics.Raycast(shoulder.position, direction, out raycastHit, rayDistance[i], 1 << LayerMask.NameToLayer("Wall")))
                {
                    Vector3 normal = -raycastHit.normal;
                    Vector3 dir = (raycastHit.point - shoulder.position);
                    dir = normal * (Vector3.Dot(dir, normal) + IKInteractivePointOffset);
                    interactivePoint = shoulder.position + dir;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                Animatiom_IKSwitch_RigConstraint(avatarIKGoal, true, interactivePoint, ValueChangeApproach.Tween);
            }
            else
            {
                Animatiom_IKSwitch_RigConstraint(avatarIKGoal,false, ValueChangeApproach.Tween);
            }
        }
        public override void OnAnimatorIK()
        {
            base.OnAnimatorIK();

            if(IKApproach.Equals(IKApproachType.OldSchool)) Animation_UpdateIK();
        }
        public void Animation_UpdateIK()
        {
            for (int i = 0; i < _IKTargets.Length; i++)
            {
                _IKTargets[i].UpdateAnimatorIK(animancer.Animator);
            }
        }
        public void Animatiom_SetIKActive(bool enable)
        {
            for (int i = 0; i < _IKTargets.Length; i++)
            {
                _IKTargets[i].SetAnimatorIK(animancer.Animator, enable ? 1 : 0, ValueChangeApproach.Immediately);
            }
        }
        public void Animatiom_IKSwitch(AvatarIKGoal _type, bool enable, Vector3 worldPosition = default, ValueChangeApproach approach = ValueChangeApproach.Tween)
        {
            for (int i = 0; i < _IKTargets.Length; i++)
            {
                if (_IKTargets[i].AvatarIKGoal != _type)
                    continue;
                _IKTargets[i].SetAnimatorIK(animancer.Animator, enable ? 1 : 0, approach);
                if (worldPosition != default)
                    _IKTargets[i].transform.position = worldPosition;
            }
        }

        //RigConstraint
        public void Animation_UpdateIK_RigConstraint()
        {
            for (int i = 0; i < _RigConstraintHandles.Length; i++)
            {
                _RigConstraintHandles[i].UpdateAnimatorIK();
            }
        }
        public void Animatiom_SetIKActive_RigConstraint(bool enable, ValueChangeApproach approach = ValueChangeApproach.Immediately)
        {
            for (int i = 0; i < _RigConstraintHandles.Length; i++)
            {
                _RigConstraintHandles[i].SetAnimatorIK(enable ? 1 : 0, ValueChangeApproach.Immediately);
            }
        }
        public void Animatiom_IKSwitch_RigConstraint(AvatarIKGoal _type, bool enable, Vector3 worldPosition = default, ValueChangeApproach approach = ValueChangeApproach.Tween)
        {
            for (int i = 0; i < _RigConstraintHandles.Length; i++)
            {
                if (_RigConstraintHandles[i].AvatarIKGoal != _type)
                    continue;
                _RigConstraintHandles[i].SetAnimatorIK(enable ? 1 : 0, approach);
                if (worldPosition != default)
                    _RigConstraintHandles[i].SetTargetPosition(worldPosition);
            }
        }
        public void Animatiom_IKSwitch_RigConstraint(AvatarIKGoal _type, bool enable,ValueChangeApproach approach = ValueChangeApproach.Tween)
        {
            for (int i = 0; i < _RigConstraintHandles.Length; i++)
            {
                if (_RigConstraintHandles[i].AvatarIKGoal != _type)
                    continue;
                _RigConstraintHandles[i].SetAnimatorIK(enable ? 1 : 0, approach);
            }
        }
        #endregion
    }

    class AttackState
    {
        public TEntity entity;
        public AnimancerState attackAnimaState;
        public string[] animaNameSet;
        public float[] exitTimeSet;
        public int index = -1;
        public Action onEnd = null;
        public AnimationLayerType layerType = AnimationLayerType.Base;

        public bool Done()
        {
            return (index > -1 && index >= animaNameSet.Length - 1) && CanExit();
        }
        public bool CanExit()
        {
            if (attackAnimaState == null)
                return true;
            else if (attackAnimaState.IsPlaying == false)
                return true;
            else if (attackAnimaState.NormalizedTime > 0.75f)
                return true;
            return false;
        }
        public void Init(TEntity entity, string[] animaNameSet, float[] exitTimeSet, Action onEnd)
        {
            this.entity = entity;
            this.attackAnimaState = null;
            this.animaNameSet = animaNameSet;
            this.exitTimeSet = exitTimeSet;
            this.onEnd = onEnd;
            index = -1;
        }
        public float GetExitTime()
        {
            if (exitTimeSet == null || exitTimeSet.Length <= 0 || index >= exitTimeSet.Length)
                return 0.5f;
            return exitTimeSet[index];
        }
        public bool CanNext()
        {
            if (animaNameSet.Length <= 0) return false;
            if (animaNameSet == null) return false;
            //
            if (attackAnimaState != null && attackAnimaState.NormalizedTime < GetExitTime()) return false;
            //
            if (Done()) return false;
            return true;
        }
        public void Next()
        {
            if (!CanNext())
                return;
            entity.animancer.Layers[(int)layerType].Weight = 1.0f;
            try
            {
                attackAnimaState = entity.Animancer_Play(
                    animaNameSet[++index % animaNameSet.Length],
                    (int)layerType
                );
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if(layerType == AnimationLayerType.Base)
                entity.animancer.Animator.applyRootMotion = true;

            if (onEnd != null)
                attackAnimaState.Events.OnEnd = onEnd;
        }
        public bool IsLayer(AnimationLayerType layerType)
        {
            return this.layerType == layerType;
        }

        public void Clear()
        {
            entity.animancer.Animator.applyRootMotion = false;
        }
    }
}
