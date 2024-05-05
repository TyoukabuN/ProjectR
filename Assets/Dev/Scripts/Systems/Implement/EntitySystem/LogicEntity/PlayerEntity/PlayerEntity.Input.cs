using PJR.Input;
using UnityEngine;

namespace PJR
{
    public partial class PlayerEntity
    {
        protected override void Init_Input()
        {
            inputHandle = new PlayerInputHandle();
            InputSystem.RegisterHandle(inputHandle);
            if (inputHandle == null)
            { 
                LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
                return;
            }
            Update_InputKCContent();
        }

        protected override void Update_InputKCContent()
        {
            if (inputHandle == null)
                return;

            var context = InputKCContent;
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move);
            context.motor ??= this.physEntity.motor;
            context.inputAxi = inputAxi;
            context.rawMoveInputVector = new Vector3(inputAxi.x, 0f, inputAxi.y);

            if (context.inputAxi.magnitude > 0)
                this.AddExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, context.rawMoveInputVector);

            if(context.inputAxi.magnitude <= 0 && this.ContainsExtraValue(EntityDefine.ExtraValueKey.SpeedModify))
                context.rawMoveInputVector = this.GetExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, context.rawMoveInputVector);

            context.moveInputVector = context.rawMoveInputVector;
            context.lookInputVector = context.rawMoveInputVector;
            if (CameraSystem.inst.mainCamera)
            {
                Quaternion CameraRotation = CameraSystem.inst.mainCamera.transform.rotation;
                Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.forward, context.motor.CharacterUp).normalized;
                if (cameraPlanarDirection.sqrMagnitude == 0f)
                {
                    cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.up, context.motor.CharacterUp).normalized;
                }

                Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, context.motor.CharacterUp);

                context.moveInputVector = cameraPlanarRotation * context.rawMoveInputVector;

                if(orientationMethod == OrientationMethod.TowardsCamera)
                    context.lookInputVector = cameraPlanarDirection;
                else if(orientationMethod == OrientationMethod.TowardsMovement)
                    context.lookInputVector = context.moveInputVector.normalized;

            }

            context.direction = context.moveInputVector;
            context.inputHandle = inputHandle;
        }
    }
}
