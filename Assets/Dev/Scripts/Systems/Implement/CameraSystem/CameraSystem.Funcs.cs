using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR
{ 
    public partial class CameraSystem : MonoSingletonSystem<CameraSystem>
    {
        private GameObject cameraRoot;
        private Camera mainCamera;
        private CinemachineBrain cinemachineBrain;

        private CinemachineVirtualCamera mainVCamera;
        public static void CreatePlayerCamera(PlayerEntity playerEntity)
        {
            if (inst.mainCamera != null)
                return;

            var root = CameraRoot;
            //MainCamera
            var mainCameraGobj = new GameObject("MainCamera");
            mainCameraGobj.transform.SetParent(root);
            inst.mainCamera = mainCameraGobj.AddComponent<Camera>();
            inst.cinemachineBrain = mainCameraGobj.AddComponent<CinemachineBrain>();
            //Main Virtual Camera
            var mainVCameraGobj = new GameObject("MainVCamera");
            mainVCameraGobj.transform.SetParent(root);
            inst.mainVCamera = mainVCameraGobj.AddComponent<CinemachineVirtualCamera>();
            //Attach to
            playerEntity = playerEntity == null ? EntitySystem.LocalPlayer : playerEntity;
            if (playerEntity != null)
            {
                inst.mainVCamera.Follow = playerEntity.physEntity.transform;
                inst.mainVCamera.LookAt = playerEntity.physEntity.transform;
                //body
                var transposer = inst.mainVCamera.AddCinemachineComponent<CinemachineTransposer>();
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                transposer.m_FollowOffset = new Vector3(0, 1.65f, -4.35f);
                //aim
                var composer = inst.mainVCamera.AddCinemachineComponent<CinemachineComposer>();
                composer.m_TrackedObjectOffset = new Vector3(0, 1, 0);
            }
        }

        public static Transform CameraRoot
        {
            get { 
                if (inst.cameraRoot == null)
                {
                    var temp = new GameObject("CameraRoot");
                    temp.transform.SetParent(inst.transform);
                    inst.cameraRoot = temp; 
                }
                return inst.cameraRoot.transform;
            }
        }
    }
}

