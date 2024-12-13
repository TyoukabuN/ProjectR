using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR.LogicUnits;
using PJR.Systems.Input;
using PJR.Systems;

namespace PJR.LogicUnits
{ 
    public class EntityInputLogicUnit : EntityLogicUnit
    {
        public InputHandle inputHandle;

        private LogicEntity dependant;
        public override LogicEntity Dependant => dependant;
        public override void OnInit(LogicEntity dependency)
        {
            this.dependant = dependency;
            inputHandle = new PlayerInputHandle();
            InputSystem.RegisterHandle(inputHandle);
            if (inputHandle == null)
            {
                LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
                return;
            }
            OnUpdate();
        }

        public override void OnUpdate()
        {
            if (inputHandle == null)
                return;

            var context = Dependant.InputKCContent;
            if (context == null)
                return;

            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move);
            context.motor ??= LogicEntity.physEntity.motor;
            context.inputAxi = inputAxi;
            context.rawMoveInputVector = new Vector3(inputAxi.x, 0f, inputAxi.y);

            if (context.inputAxi.magnitude > 0)
                LogicEntity.AddExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, context.rawMoveInputVector);

            if (context.inputAxi.magnitude <= 0 && LogicEntity.ContainsExtraValue(EntityDefine.ExtraValueKey.SpeedModify))
                context.rawMoveInputVector = LogicEntity.GetExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, context.rawMoveInputVector);

            context.moveInputVector = context.rawMoveInputVector;
            context.lookInputVector = context.rawMoveInputVector;
            if (CameraSystem.MainCamera)
            {
                Quaternion CameraRotation = CameraSystem.MainCamera.transform.rotation;
                Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.forward, context.motor.CharacterUp).normalized;
                if (cameraPlanarDirection.sqrMagnitude == 0f)
                {
                    cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.up, context.motor.CharacterUp).normalized;
                }

                Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, context.motor.CharacterUp);

                context.moveInputVector = cameraPlanarRotation * context.rawMoveInputVector;

                if (LogicEntity.OrientationMethod == OrientationMethod.TowardsCamera)
                    context.lookInputVector = cameraPlanarDirection;
                else if (LogicEntity.OrientationMethod == OrientationMethod.TowardsMovement)
                    context.lookInputVector = context.moveInputVector.normalized;

            }

            context.direction = context.moveInputVector;
            context.inputHandle = inputHandle;
        }
    }
}
