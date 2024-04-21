using UnityEngine;
using PJR.Input;
using PJR.ScriptStates.Player;
using System.Runtime.InteropServices.ComTypes;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        protected void Init_State()
        {
            scriptStateMachine = new PlayerScriptStateMachine(this);
            scriptStateMachine.Init();
            physEntity.onUpdateVelocity += OnUpdateVelocity;
            physEntity.onUpdateRotation += OnUpdateRotation;
        }
        protected void Update_State()
        {
            scriptStateMachine?.Update();
        }

        protected void Destroy_State()
        {
            physEntity.onUpdateVelocity -= OnUpdateVelocity;
            physEntity.onUpdateRotation -= OnUpdateRotation;
        }
        protected void OnUpdateRotation(KCContext context)
        {
            if (scriptStateMachine == null)
                return;

            CopyInputKCContent(context);

            var currentRotation = context.currentRotation;

            if (context.lookInputVector.sqrMagnitude > 0f && physicsConfig.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(context.motor.CharacterForward, context.lookInputVector, 1 - Mathf.Exp(-physicsConfig.OrientationSharpness * context.deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, context.motor.CharacterUp);
            }

            Vector3 currentUp = (currentRotation * Vector3.up);
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-physicsConfig.OrientationSharpness * context.deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;

            context.currentRotation = currentRotation;

            scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        }
        protected void OnUpdateVelocity(KCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            CopyInputKCContent(context);

            context.physConfig = physicsConfig;

            //状态的速度控制
            scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
            ////额外速度控制
            this.UpdateExtraVelocity(context.deltaTime,out var extraVec3);
            if (extraVec3.y > 0)
                context.motor.ForceUnground();
            context.currentVelocity += extraVec3;
        }

    }
}
