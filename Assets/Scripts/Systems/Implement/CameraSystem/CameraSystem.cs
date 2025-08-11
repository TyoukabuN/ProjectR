using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace PJR.Systems
{ 
    public partial class CameraSystem : MonoSingletonSystem<CameraSystem>
    {
        static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        public static CinemachineVirtualCamera activationCamera;
        public static void Register(CinemachineVirtualCamera camera)
        {
            if (camera == null && cameras.Contains(camera))
                return;
            cameras.Add(camera);
        }

        public static void Unregister(CinemachineVirtualCamera camera)
        {
            if (camera == null && !cameras.Contains(camera))
                return;
            cameras.Remove(camera);
        }
        static void CheckVaild()
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                var camera = cameras[i];
                if (camera != null)
                    continue;
                cameras.RemoveAt(i--);
            }
        }
        public static bool SwitchCamera(CinemachineVirtualCamera camera)
        {
            if (camera == null)
                return false;
            if (IsActiveCamera(camera))
            {
                camera.Priority = 10;
                return true;
            }
            CheckVaild();
            camera.Priority = 10;
            activationCamera = camera;
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameras[i] == null || cameras[i] == activationCamera || cameras[i].Priority == 0)
                    continue;
                cameras[i].Priority = 0;
            }
            return true;
        }

        public static bool IsActiveCamera(CinemachineVirtualCamera camera)
        {
            return camera != null && camera == activationCamera;
        }

        public static CinemachineVirtualCamera GetActiveCamera()
        {
            return activationCamera;
        }

        public static void SetFollow(Transform trans)
        {
            if (trans == null)
                return;
            foreach (var camera in cameras)
            {
                //var framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
                if (camera)
                {
                    camera.Follow = trans;
                }
            }
        }
    }
}

