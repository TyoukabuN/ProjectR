using PJR.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.AddressableAssets.Build.BuildPipelineTasks.GenerateLocationListsTask;

namespace PJR
{
    /// <summary>
    /// 玩家控制方法
    /// </summary>
    public static class PlayerKCCFunc
    {

        public static bool GetSpeedModifyVelMagnitude(KCContext ctx,float defalutValue,out float value,out float orientationSharpness)
        {
            if (ctx.logicEntity.TryGetExtraValue<SpeedModifyParam>(EntityDefine.ExtraValueKey.SpeedModify, null, out var valueRef))
            {
                value = valueRef.speed;
                orientationSharpness = valueRef.orientationSharpness;
                return true;
            }
            value = defalutValue;
            orientationSharpness = ctx.cfg.OrientationSharpness;
            return false;


        }
        /// <summary>
        /// 地面上移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void GroundedMove(KCContext ctx)
        {
            var output = ctx.currentVelocity;
            //
            //当前速度
            var inputVelocity = ctx.currentVelocity;
            //转向系数
            var orientationSharpness = ctx.cfg.OrientationSharpness;
            //当前速度大小
            var currentVelocityMagnitude = ctx.currentVelocity.magnitude;
            //目标速度
            float targetVelocityMagnitude = ctx.cfg.MaxGroundedMoveSpeed;
            if (ctx.inputHandle.HasAnyFlag(RegisterKeys.Run))
                targetVelocityMagnitude = ctx.cfg.ACCMaxGroundedMoveSpeed;

            var anyModify = GetSpeedModifyVelMagnitude(ctx, targetVelocityMagnitude,out targetVelocityMagnitude,out orientationSharpness);
            //输入方向
            Vector3 inputAxiVec3 = ctx.moveInputVector;
            bool AnyInputAxi = inputAxiVec3.magnitude > 0;
            //当前有些地面法线
            Vector3 effectiveGroundNormal = ctx.motor.GroundingStatus.GroundNormal;

            ////计算输入方向
            var currentDir = ctx.motor.GetDirectionTangentToSurface(inputVelocity, effectiveGroundNormal);// * currentVelocityMagnitude;
            Vector3 reorientedInputDir = ctx.motor.GetDirectionTangentToSurface(inputAxiVec3, effectiveGroundNormal);

            Vector3 finalDir = Vector3.Lerp(currentDir, reorientedInputDir, 1f - Mathf.Exp(-orientationSharpness * ctx.deltaTime));


            ////计算速度大小
            float finalVelocityMagnitude = currentVelocityMagnitude;
            if (finalVelocityMagnitude > targetVelocityMagnitude)
            {
                //减速
                finalVelocityMagnitude = Mathf.Lerp(currentVelocityMagnitude, targetVelocityMagnitude, 1f - Mathf.Exp(-ctx.cfg.SpeedDamping * ctx.deltaTime));
            }
            else if(AnyInputAxi)
            {
                //加速
                finalVelocityMagnitude += ctx.deltaTime * ctx.cfg.GroundedMoveACCSpeed;
            }

            if (anyModify)
                finalVelocityMagnitude = targetVelocityMagnitude;

            ctx.currentVelocity = finalDir * finalVelocityMagnitude;
        }

        /// <summary>
        /// 地面上移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void InHurt(KCContext ctx)
        {
            var output = ctx.currentVelocity;
            output += ctx.cfg.Gravity * ctx.deltaTime;
            output *= (1f / (1f + (ctx.cfg.SpeedDampingInHurt * ctx.deltaTime)));

            ctx.currentVelocity = output;
        }

        /// <summary>
        /// 进入空中
        /// </summary>
        /// <param name="ctx"></param>
        public static void EnterToAir(KCContext ctx)
        {
            var output = ctx.currentVelocity;
            var motor = ctx.motor;

            float JumpUpSpeed = ctx.cfg.JumpUpSpeed;

            Vector3 jumpDirection = ctx.motor.CharacterUp;
            if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
            {
                jumpDirection = ctx.motor.GroundingStatus.GroundNormal;
            }

            //output += (jumpDirection * JumpUpSpeed) - Vector3.Project(output, motor.CharacterUp);
            output.y = 0;
            output += (jumpDirection * JumpUpSpeed);
            ctx.currentVelocity = output;

            motor.ForceUnground();
        }

        /// <summary>
        /// 空中移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void OnAir(KCContext ctx)
        {
            var output = ctx.currentVelocity;
            var cfg = ctx.cfg;

            Vector3 addedVelocity = ctx.moveInputVector * cfg.AirAccelerationSpeed * ctx.deltaTime;
            float maxAirMoveSpeed = cfg.MaxAirMoveSpeed;
            bool anyModify = GetSpeedModifyVelMagnitude(ctx, maxAirMoveSpeed, out float velMagnitude, out float orientationSharpness);
            if (anyModify)
            {
                maxAirMoveSpeed = velMagnitude;
                addedVelocity = ctx.moveInputVector.normalized * velMagnitude;
            }

            if (ctx.moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(output, ctx.motor.CharacterUp);

                // Limit air velocity from inputs
                if (currentVelocityOnInputsPlane.magnitude < maxAirMoveSpeed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, maxAirMoveSpeed);
                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                }
                else
                {
                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    {
                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }
                }

                // Prevent air-climbing sloped walls
                if (ctx.motor.GroundingStatus.FoundAnyGround)
                {
                    if (Vector3.Dot(output + addedVelocity, addedVelocity) > 0f)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(ctx.motor.CharacterUp, ctx.motor.GroundingStatus.GroundNormal), ctx.motor.CharacterUp).normalized;
                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                    }
                }

                // Apply added velocity
                output += addedVelocity;
            }
            //重力
            output += cfg.Gravity * ctx.deltaTime;
            output *= (1f / (1f + (cfg.SpeedDamping * ctx.deltaTime)));
            ctx.currentVelocity = output;

            if (anyModify && velMagnitude <= 0)
                ctx.currentVelocity = Vector3.zero;
        }


        /// <summary>
        /// 通用旋转
        /// </summary>
        /// <param name="context"></param>
        public static void CommonRotation(KCContext context)
        {
            var orientationSharpness = context.cfg.OrientationSharpness;
            var dashing = GetSpeedModifyVelMagnitude(context, 0, out var targetVelocityMagnitude, out orientationSharpness);
            //输入方向
            var currentRotation = context.currentRotation;

            if (context.lookInputVector.sqrMagnitude > 0f && orientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(context.motor.CharacterForward, context.lookInputVector, 1 - Mathf.Exp(-orientationSharpness * context.deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, context.motor.CharacterUp);
            }

            Vector3 currentUp = (currentRotation * Vector3.up);
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-orientationSharpness * context.deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;

            context.currentRotation = currentRotation;
        }
    }
}
