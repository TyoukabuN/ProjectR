using KinematicCharacterController;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoterOption : MonoBehaviour
{
    [InlineButton("SetMovementCollisionsSolvingActivation", "Set")]
    [LabelText("处理移动碰撞")]
    public bool solveMovementCollisions;

    [InlineButton("SetHasPlanarConstraint", "Set")]
    [LabelText("平坦限制")]
    public bool HasPlanarConstraint;

    private KinematicCharacterMotor _motor;
    private KinematicCharacterMotor motor => _motor ??= GetComponent<KinematicCharacterMotor>();
    public void SetMovementCollisionsSolvingActivation()
    {
        motor?.SetMovementCollisionsSolvingActivation(solveMovementCollisions);
    }

    public void SetHasPlanarConstraint()
    {
        motor.HasPlanarConstraint = HasPlanarConstraint;
    }


    [Button("MoveCharacter一步")]
    public void MoveOneStep()
    {
        motor?.MoveCharacter(transform.position + transform.forward);
    }
}
