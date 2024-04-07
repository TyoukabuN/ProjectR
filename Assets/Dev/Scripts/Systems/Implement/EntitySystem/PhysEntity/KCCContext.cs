using KinematicCharacterController;
using PJR.Input;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{ 
    public class KCCContext 
    {
        //refs
        public PhysEntity physEntity = null;
        public KinematicCharacterMotor motor = null;
        //input value
        public Vector2 inputAxi = Vector2.zero;
        public Vector3 direction = Vector2.one;
        public Dictionary<InputKey,bool> inputKeyActive = new Dictionary<InputKey,bool>(3);
        //velocity
        public Vector3 inputVelocity = Vector3.zero;
        public Vector3 outputVelocity = Vector3.zero;
        //quaternion
        public Quaternion inputRotation = Quaternion.identity;
        public Quaternion outputRotation = Quaternion.identity;
        //time
        public float deltaTime = 0f;
        public void ExecuteCommandBuffer()
        { 
        }
    }
    public class KCCCommandBuffer
    {
        //velocity
        public Vector3 outputVelocity;
        //quaternion
        public Quaternion outputRotation;
    }
}
