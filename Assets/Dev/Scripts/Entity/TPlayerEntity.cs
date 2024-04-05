using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Define = PJR.TEntityDefine;
using System.Text;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class TPlayerEntity : StateMachineEntity , INumericalControl, IActionControl
    {
        //motion require
        private Rigidbody _rigidbody;
        //
        [LabelText("跑步系数")] public float runFactor = 2.33f;

        [LabelText("步行系数")] public float walkFactor = 1f;
        [LabelText("步行速度")] public float walkSpeedMagnitude = 3f;
        //
        [LabelText("跳跃高度")] public float jumpHeight = 2f;
        [LabelText("可跳跃次数")] public int jumpableTimes = 2;

        [HideInInspector] public int jumpCounter = 0;
        //
        [LabelText("冲刺系数")] public float dashFactor = 1.0f;
        [LabelText("冲刺")] public float dashForce = 10f;
        //[LabelText("冲刺力方向")] public Vector2 dashDirection = Vector2.right;
        [LabelText("冲刺持续时间")] public float dashDuration = 0.8f;
        [HideInInspector] public float dashCounter = 0f;
        [HideInInspector] public float dashCounterNormalize = 0f;

        [HideInInspector] private Vector3 m_floorUp = Vector3.up;
        public Vector3 floorUp
        {
            get
            {
                if (!Grounded) return Vector3.up;
                return contactNormal;
            }
            set
            {
                m_floorUp = value;
            }
        }
        public GameObject cameraSpot;
        private Vector3 m_tempVec3 = Vector3.zero;
        public Vector3 cameraForward
        {
            get {
                Vector3 forward = cameraSpot.transform.forward;
                m_tempVec3.x = forward.x;
                m_tempVec3.y = 0f;
                m_tempVec3.z = forward.z;
                return m_tempVec3.normalized;
            }
        }
        public Vector3 right => Vector3.Cross(Vector3.up, cameraForward).normalized;
        public Vector3 forward {
            get {
                if (!Grounded) {
                    return cameraForward;
                }
                return Vector3.Cross(Vector3.Cross(floorUp, cameraForward), floorUp).normalized;
            }
        }



        private bool m_isLeftControlClick = false;
        private bool m_isSpaceClick = false;
        private bool m_isDownArrowClick = false;
        private bool m_isUpArrowClick = false;
        private bool m_isTweeningPosY = false;

        public Action<bool> onCurrentFloorColliderChange;
        [HideInInspector] public int lastFloorCollider2D = -1;
        [HideInInspector] public bool m_lastFloorColliderIsEdge = false;

        protected override void Awake()
        {
            base.Awake();
            Awake_Input();
            Awake_State();
        }


        protected override void Start()
        {
            base.Start();
        }
        public override void OnDestroy()
        {
            this.UnregisterExtraVelocity();
            this.UnregisterExtraVelocityMap();
            this.UnregisterExtraActionMap();
        }

        protected override void Update()
        {
            base.Update();
            
            if (!IsVaild())
                return;

            //Update_Input();
            Update_State();
            Update_Animation();
        }
        protected void LateUpdate()
        {
            Update_Input();
        }

        private Vector3 velocity;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Collision_UpdateState();
            velocity = _rigidbody.velocity;
            //un hang
            //if (m_isUpArrowClick)
            //{
            //    m_isUpArrowClick = false;

            //    if (CanHang() && IsHanging())
            //    {
            //        DOHang();
            //    }
            //}

            UpdateDashProperty();

            Collision_CheckStayingFloor(ref m_floorUp);

            if(!mainAnimator.applyRootMotion)
                velocity += GetHorizontalVelocity(Time.fixedDeltaTime);

            //velocity += GetExtraVelocity(Time.fixedDeltaTime);

            velocity += GetJumpVelocity();

            //UpdateJumpAnimationProperty();


            _rigidbody.velocity = velocity;

            FixedUpdate_Animation();

            Collision_ClearState();
        }

        private void UpdateDashProperty()
        {
            if (m_isLeftControlClick)
            {
                m_isLeftControlClick = false;
                dashCounter = dashDuration;
                //rig.AddForce(DushDirection * DushForce, ForceMode2D.Impulse);
                this.ExtraActionMapAdd(TPlayerActionType.CreateGhost, dashDuration);
                //transform.localPosition = transform.localPosition + (Vector3)(dashDirection * 0.05f);
            }
            if (IsDashing())
            {
                dashCounter -= Time.fixedDeltaTime;
                //跳跃状态下,冲刺效果不结束
                if (IsJumping())
                {
                    if (dashCounter <= 0)
                    {
                        dashCounter += Time.fixedDeltaTime;
                    }
                    var gaction = this.GetExtraActionMap(TPlayerActionType.CreateGhost);
                    if (gaction != null && (gaction.counter - Time.fixedDeltaTime) <= 0)
                    {
                        gaction.counter += Time.fixedDeltaTime;
                    }
                }

            }

            dashCounterNormalize = Mathf.Clamp(dashCounter / dashDuration, 0, 1);
        }

        private void UpdateJumpAnimationProperty()
        {
            int jumpVal = 1;
            if (!IsJumping() || Grounded)
                jumpVal = 0;

            int jumpBlend = 0;
            if (m_isTweeningPosY)
            {
                jumpBlend = -2;
            }
            else if (_rigidbody.velocity.y > 0)
            {
                jumpBlend = 1;
            }
            else if (!Grounded)
            {
                OnFallBegin();
                jumpBlend = -1;
            }

            if (m_isTweeningPosY)
            {
                jumpVal = 1;
                jumpBlend = -2;
            }

            Animator_SetInt(Define.ANIME_HASH_JUMP_STATE, jumpVal);
            Animator_SetInt(Define.ANIME_HASH_JUMP_INT_VALUE, jumpBlend);
        }

        public void SetJumpCount(int value)
        {
            jumpCounter = value;
            //Animator_SetInt(Define.ANIME_HASH_JUMP_COUNT, jumpCounter);
        }

        //检查看是下落
        public bool m_fallBeginTrigger = true;
        public Action onFallBegin;
        public void OnFallBegin()
        {
            if (onFallBegin == null)
                return;
            if (!m_fallBeginTrigger)
                return;
            m_fallBeginTrigger = false;
            try
            {
                onFallBegin.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        public void ResetFallBeginTrigger()
        {
            m_fallBeginTrigger = true;
        }

        [SerializeField, Range(0f, 100f)]
        float maxAcceleration = 10f;
        [SerializeField, Range(0f, 100f)]
        float maxAirAcceleration = 2f;
        [SerializeField, Range(0f, 100f)]
        float slopeBiasAirAcceleration = 1.3f;
        [SerializeField, Range(0f, 1f)]
        float originVelocityRate = 0.75f;

        private float acceleration => Grounded ? maxAcceleration : maxAirAcceleration;

        Vector3 ProjectOnContactPlane(Vector3 vector)
        {
            return vector - contactNormal * Vector3.Dot(vector, contactNormal);
        }
        public Vector3 GetHorizontalVelocity(float deltaTimeForUpdate = 0)
        {
            float runAddition = IsRunning() ? runFactor : 1;
            float factor = walkSpeedMagnitude * walkFactor * runAddition + dashCounterNormalize * dashForce * dashFactor;

            Vector3 xAxis = this.right;
            Vector3 zAxis = this.forward;
            Vector3 yAxis = contactNormal;

            Vector3 desiredVelocity = new Vector3(inputAxi.x, 0, inputAxi.y) + debugVec3;
            desiredVelocity.y = -Vector3.Dot(desiredVelocity.normalized, yAxis);
            desiredVelocity  *= factor;
            //
            Vector3 originVelocity = desiredVelocity * originVelocityRate;


            float currentX = Vector3.Dot(velocity, xAxis);
            float currentZ = Vector3.Dot(velocity, zAxis);
            //
            float currentY = Vector3.Dot(velocity, yAxis);


            float dashAccelerationFactor = (dashCounterNormalize > 0 ? 1.5f : 0.0f) * dashFactor;
            float maxSpeedChange = acceleration * (1 + dashAccelerationFactor) * Time.deltaTime;

            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
            //
            float newY = Mathf.MoveTowards(currentY, desiredVelocity.y, slopeBiasAirAcceleration * Time.deltaTime);


            Vector3 addition = Vector3.zero;
            addition.x = newX - currentX;
            addition.z = newZ - currentZ;

            return (xAxis * (addition.x) + zAxis * (addition.z) + yAxis * (newY - currentY));
        }
        public Vector3 debugVec3 = Vector3.zero;
        public Vector3 GetJumpVelocity()
        {
            //jump
            if (m_isSpaceClick)
            {
                m_isSpaceClick = false;
                SetJumpCount(jumpCounter + 1);
                Collision_ClearStayingFloor();
                //Animation_OnJumpStart();

                this.ExtraActionMapRemove(TPlayerActionType.KeepOnFloor);

                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
                float alignedSpeed = Vector3.Dot(velocity, contactNormal);
                if (alignedSpeed > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                }
                //return contactNormal * jumpSpeed;
                return Vector3.up * jumpSpeed;
            }
            return Vector3.zero;
        }


        public float GetGravitySign()
        {
            return m_hanging ? -1f : 1f;
        }

        public bool DOHang(bool immediately = false)
        {
            if (true)
                return false;
        }

        public void ClearTweenerPosY()
        {
            this.ExtraActionMapRemove(TPlayerActionType.TweeningPosY);
            SetAsDefaultColliderMask();
            m_isTweeningPosY = false;
            m_hanging = false;
            UpdateModelFlip();
        }
        private Vector3 m_InverseYV3 = new Vector3(1, -1, 1);
        private Vector3 m_NormalYV3 = new Vector3(1, 1, 1);
        public void UpdateModelFlip()
        {
            spriteRenderer.flipY = m_hanging ? true : false;
            if (ModelRoot)
            {
                ModelRoot.localPosition = Vector3.Scale(modelRoot_localPosition,m_hanging ? m_InverseYV3 : m_NormalYV3);
                ModelRoot.localScale = Vector3.Scale(modelRoot_localScale, m_hanging ? m_InverseYV3 : m_NormalYV3); ;
            }
        }
        //
      
        public AfterImageEffects spriteGhostCreator;
        public void GhostSwitch(bool enabled)
        {
            if (spriteGhostCreator == null)
                spriteGhostCreator = gameObject.GetComponent<AfterImageEffects>();
            if (spriteGhostCreator == null)
                spriteGhostCreator = gameObject.AddComponent<AfterImageEffects>();
            if (enabled)
            { 
                spriteGhostCreator.Init();
            }
            spriteGhostCreator._OpenAfterImage = enabled;
        }
        

        [HideInInspector] public static string[] defaultColliderMaskNames = new string[] { "Default", "Entity", "Floor" };
        [HideInInspector] public static string[] translateColliderMaskNames = new string[] { "Entity" };
        public void SetAsDefaultColliderMask()
        {
            if (capsuleCollider == null)
                return;
            capsuleCollider.isTrigger = false;
            ChangeColliderMask(new string[] { "Default", "Entity", "Floor" });
        }
        public void SetAsTranslateColliderMask()
        {
            if (capsuleCollider == null)
                return;
            capsuleCollider.isTrigger = true;
            ChangeColliderMask(new string[] { "Default", "Entity", "Floor" });
        }

        public bool isDebug = true;

        #region Editor

#if UNITY_EDITOR
        private GUIStyle GizmosGUIStyle = null;
        StringBuilder builder;

        void OnDrawGizmos()
        {
            if (!isDebug)
                return;
            if (GizmosGUIStyle == null)
            {
                GUIStyle style = new GUIStyle();
                style.richText = true;
                GizmosGUIStyle = style;
            }
            if (builder == null) { 
                builder = new StringBuilder();
            }
            builder.Clear();

            builder.AppendLine(string.Format("<color=red>跳跃:{0}/{1}</color>", jumpCounter.ToString(), jumpableTimes.ToString()));
            builder.AppendLine(string.Format("<color=red>着地:{0}</color>", Grounded.ToString()));
            builder.AppendLine(string.Format("<color=red>移动输入:{0}</color>", inputAxi.ToString()));
            builder.AppendLine(string.Format("<color=red>速度:{0}</color>", _rigidbody == null ?0:_rigidbody.velocity.ToString()));
            builder.AppendLine(string.Format("<color=red>XZAnima:{0}</color>", inputAxi));
            builder.AppendLine(string.Format("<color=red>状态:{0}</color>", currentEState.ToString()));

            Handles.Label(transform.position, builder.ToString(), GizmosGUIStyle);
            Handles.color = Color.red;

            Gizmos.color = Color.blue;
            var from = transform.position;
            from.y += 0.5f;
            Gizmos.DrawLine(from, from + forward);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + right);
            //Gizmos.DrawWireSphere(transform.position + groundedOffset, groundedRadius);


            Animation_OnDrawGizmos();
        }
#endif

        public Rect GetRect(ref float x, ref float y, ref float w, ref float h)
        {
            var rect = new Rect(x, y, w, h);
            y += h;
            return rect;
        }
        public Rect GetRect(ref Vector2 pos, ref Vector2 size)
        {
            var rect = new Rect(pos.x, pos.y, size.x, size.y);
            pos.y += size.y;
            return rect;
        }
        public Rect GetRect(ref Vector2 pos, float w, float h)
        {
            var rect = new Rect(pos.x, pos.y, w, h);
            pos.y += h;
            return rect;
        }
    }
#endregion 
}