using KinematicCharacterController;
using PJR.Input;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace PJR
{ 
    public class KCContext 
    {
        //refs
        public PhysEntity physEntity = null;
        public KinematicCharacterMotor motor = null;
        //config
        public EntityPhysicsConfigAsset physConfig = null;
        //input value
        public InputHandle inputHandle = null;
        public Vector2 inputAxi = Vector2.zero;
        public Vector3 inputAxiVec3 = Vector3.zero;
        public Vector3 direction = Vector3.one;
        //velocity
        public Vector3 currentVelocity = Vector3.zero;
        //quaternion
        public Quaternion inputRotation = Quaternion.identity;
        public Quaternion outputRotation = Quaternion.identity;
        //time
        public float deltaTime = 0f;
        //field
        public EntityPhysicsConfigAsset cfg => physConfig;
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
