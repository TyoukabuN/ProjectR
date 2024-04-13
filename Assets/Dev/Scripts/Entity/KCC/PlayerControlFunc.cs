using PJR.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    /// <summary>
    /// 玩家控制方法
    /// </summary>
    public static class PlayerControlFunc
    {
        /// <summary>
        /// 地面上移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void GroundedMove(KCCContext ctx)
        {
            var output = ctx.inputVelocity;
            var currentVelocityMagnitude = ctx.inputVelocity.magnitude;
            float targetVelocity = ctx.cfg.MaxGroundedMoveSpeed;
            if (ctx.inputHandle.HasAnyFlag(RegisterKeys.Run))
                targetVelocity = ctx.cfg.ACCMaxGroundedMoveSpeed;
            Vector3 inputAxiVec3 = ctx.inputAxiVec3;

            Vector3 effectiveGroundNormal = ctx.motor.GroundingStatus.GroundNormal;

            output = ctx.motor.GetDirectionTangentToSurface(output, effectiveGroundNormal) * currentVelocityMagnitude;

            Vector3 reorientedInput = ctx.motor.GetDirectionTangentToSurface(inputAxiVec3, effectiveGroundNormal);

            Vector3 targetMovementVelocity = reorientedInput * targetVelocity;

            output = Vector3.Lerp(output, targetMovementVelocity, 1f - Mathf.Exp(-15 * ctx.deltaTime));
            ctx.outputVelocity = output;

            ////get taraget speed
            //if (ctx.inputHandle.HasAnyFlag(RegisterKeys.Run))
            //    targetVelocity = ctx.cfg.ACCMaxGroundedMoveSpeed;

            //if (currentVelocityMagnitude > ctx.cfg.MaxGroundedMoveSpeed)
            //{
            //    //dampping
            //    currentVelocityMagnitude = Mathf.Lerp(currentVelocityMagnitude, targetVelocity, ctx.cfg.SpeedDamping);
            //}
            //else
            //{
            //    //acc
            //    currentVelocityMagnitude += ctx.deltaTime * ctx.cfg.GroundedMoveACCSpeed;
            //}

           // ctx.outputVelocity = ctx.direction * currentVelocityMagnitude;
        }

        /// <summary>
        /// 进入空中
        /// </summary>
        /// <param name="ctx"></param>
        public static void EnterToAir(KCCContext ctx)
        {
            var output = ctx.inputVelocity;
            var motor = ctx.motor;

            float JumpUpSpeed = ctx.cfg.JumpUpSpeed;

            Vector3 jumpDirection = ctx.motor.CharacterUp;
            if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
            {
                jumpDirection = ctx.motor.GroundingStatus.GroundNormal;
            }

            output += (jumpDirection * JumpUpSpeed) - Vector3.Project(output, motor.CharacterUp);
            ctx.outputVelocity = output;

            motor.ForceUnground();
        }

        /// <summary>
        /// 空中移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void OnAir(KCCContext ctx)
        {
            var output = ctx.inputVelocity;
            var cfg = ctx.cfg;

            if (ctx.inputAxiVec3.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = ctx.inputAxiVec3 * cfg.AirAccelerationSpeed * ctx.deltaTime;

                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(output, ctx.motor.CharacterUp);

                // Limit air velocity from inputs
                if (currentVelocityOnInputsPlane.magnitude < cfg.MaxAirMoveSpeed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, cfg.MaxAirMoveSpeed);
                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                }
                else
                {
                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    {
                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }
                    else
                    {
                        Debug.Log(321);
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
            ctx.outputVelocity = output;
        }
    }
}
