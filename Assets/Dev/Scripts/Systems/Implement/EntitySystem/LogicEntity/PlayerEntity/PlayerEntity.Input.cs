using PJR.Systems.Input;
using PJR.Systems;
using UnityEngine;

namespace PJR
{
    public partial class PlayerEntity
    {
        protected override void Init_Input()
        {

        }

        protected override void Update_InputKCContent()
        {
            if (inputHandle == null)
                return;

            var context = InputKCContent;
            if (context == null)
                return;

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

                if(OrientationMethod == OrientationMethod.TowardsCamera)
                    context.lookInputVector = cameraPlanarDirection;
                else if(OrientationMethod == OrientationMethod.TowardsMovement)
                    context.lookInputVector = context.moveInputVector.normalized;

            }

            context.direction = context.moveInputVector;
            context.inputHandle = inputHandle;
        }
    }
}
