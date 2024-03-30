using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace TinyGame
{
    [RequireComponent(typeof(PlayerInput))]
    public partial class TPlayerEntity : StateMachineEntity, INumericalControl, IActionControl
    {
        protected bool m_lastCanUp = false;
        public Action<bool> onCanUpChange;

        protected float rotationPower = 0.15f;

        protected Vector2 inputAxi;
        protected Vector2 mouseDelta;
        protected float runValue;

        private PlayerInput playerInput;
        protected void Awake_Input()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
            if (playerInput.actions == null)
            {
                return;
            }
  
            RegisterPlayerInputCallback("Move"  ,OnMove ,OnMove);
            RegisterPlayerInputCallback("Look"  ,OnLook ,OnLook);
            RegisterPlayerInputCallback("Run"   ,OnRun  ,OnRun);
            RegisterPlayerInputCallback("Jump"  ,OnJump ,null);
            RegisterPlayerInputCallback("Dash"  ,OnDash , null);
            RegisterPlayerInputCallback("Fire"  ,OnFire , null);
        }

        #region Input System Message

        protected void RegisterPlayerInputCallback(string actionNameOrId, Action<CallbackContext> performed, Action<CallbackContext> canceled)
        {
            var action = playerInput.actions.FindAction(actionNameOrId);
            if(performed != null) action.performed += performed;
            if (canceled != null) action.canceled += canceled;
        }
        protected void OnMove(CallbackContext callback)
        {
            inputAxi = callback.ReadValue<Vector2>();
            if (inputAxi.magnitude > 0)
            {
                State_Change(EPlayerState.Walk);
            }
            else
            {
                State_Change(EPlayerState.Stand);
            }
        }

        protected void OnLook(CallbackContext callback)
        {
            mouseDelta = callback.ReadValue<Vector2>();
        }

        protected void OnRun(CallbackContext value)
        {
            runValue = value.ReadValue<float>();
            if (IsRunning())
            {
                State_Change(EPlayerState.Running);
            }
            else
            {
                State_Change(EPlayerState.Walk);
            }
        }

        protected void OnJump(CallbackContext value)
        {
            if (CanJump())
            {
                if (Input_Jump())
                    State_Change(EPlayerState.Jump_Begin);
            }
            else if (IsHanging())
            {
                Input_UnHang();
            }
        }
        protected void OnDash(CallbackContext value)
        {
            //TODO:先禁掉冲刺
            if (true)
                return;
            Input_Dash();
        }
        protected void OnFire(CallbackContext value)
        {
            Input_Attack();
        }
        #endregion

        protected void Update_Input() {

            if (!CanOperate())
            {
                m_isDownArrowClick = false;
                m_isLeftControlClick = false;
                m_isSpaceClick = false;
                m_isUpArrowClick = false;
                return;
            }

            //计算cameraSpot的旋转
            cameraSpot.transform.rotation *= Quaternion.AngleAxis(mouseDelta.x * rotationPower, Vector3.up);
            cameraSpot.transform.rotation *= Quaternion.AngleAxis(-mouseDelta.y * rotationPower, Vector3.right);

            var angles = cameraSpot.transform.localEulerAngles;
            angles.z = 0;

            var angle = cameraSpot.transform.localEulerAngles.x;

            //限制仰/俯角
            if (angle > 180 && angle < 340)
            {
                angles.x = 340;
            }
            else if (angle < 180 && angle > 40)
            {
                angles.x = 40;
            }
            cameraSpot.transform.localEulerAngles = angles;

            //因为cameraSpot跟transfrom的Parent关系，将cameraSpot的y轴旋转应用到transfrom
            transform.rotation = Quaternion.Euler(0, cameraSpot.transform.rotation.eulerAngles.y, 0);
            //cameraSpot只需要关注x轴旋转
            cameraSpot.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        }
        public bool canOperate = true;

        public bool CanOperate()
        {
            //TODO:CanOperate
            //if (!IsVaild())
            //    return false;
            //if (!TinyGameManager.instance.isRunning)
            //    return false;
            //if (!TinyGameManager.playerControllable)
            //    return false;
            return canOperate;
        }

        public bool Input_Attack()
        {
            if (!CanOperate())
                return false;
            if(!CanAttack())
                return false;
            Animation_Attack();
            return true;
        }
        public bool Input_UnHang()
        {
            if (!CanOperate())
                return false;
            m_isUpArrowClick = true;
            return true;
        }
        public bool Input_Dash()
        {
            if (!CanOperate())
                return false;
            if (!CanDash())
                return false;
            m_isLeftControlClick = true;
            return true;
        }
        public bool Input_Jump()
        {
            if (!CanOperate())
                return false;
            if (!CanJump())
                return false;
            m_isSpaceClick = true;
            return true;
        }
        public bool IsRunning()
        {
            return runValue > 0;
        }
        public bool IsMoving()
        {
            return inputAxi.magnitude > 0;
        }

        public bool CanAttack() 
        {
            return Grounded;
        }
        public bool CanDash()
        {
            if (IsJumping())
                return false;
            return !IsDashing();
        }
        public bool IsDashing()
        {
            return dashCounter > 0;
        }
        public void StopDash()
        {
            dashCounter = 0;
        }
        public bool IsJumping()
        {
            //return rig.velocity.y < -0.05f || rig.velocity.y > 0.05f;
            return !Grounded;
        }
        public bool CanJump()
        {
            return jumpCounter < jumpableTimes && !IsHanging();
        }

        public bool CanHang()
        {
            return false;// IsGrounded() && IsEdgeCollider2DGround();
        }

        private bool m_hanging = false;
        public bool IsHanging()
        {
            return m_hanging;
        }


    }
}
