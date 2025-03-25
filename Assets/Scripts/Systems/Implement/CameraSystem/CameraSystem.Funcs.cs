using Cinemachine;
using UnityEngine;
using PJR.ClassExtension;

namespace PJR.Systems
{ 
    public partial class CameraSystem : MonoSingletonSystem<CameraSystem>
    {
        public static Camera MainCamera => instance?.mainCamera;

        public GameObject cameraRoot;
        public Camera mainCamera;
        public CinemachineBrain cinemachineBrain;
        public CinemachineVirtualCamera mainVCamera;

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
                transposer.m_FollowOffset = new Vector3(0, 1.65f, 0f);
                //aim
                var composer = inst.mainVCamera.AddCinemachineComponent<CinemachineComposer>();
                composer.m_TrackedObjectOffset = new Vector3(0, 1, 0);
                //path
                CinemachineSmoothPath path = null;
                var sceneRoot = SceneSystem.instance.sceneRoot;
                if (sceneRoot != null)
                {
                    Transform raceCameraPathGobj = sceneRoot.transform.Find("Cameras/RaceCameraPath");
                    if (raceCameraPathGobj != null)
                        path = raceCameraPathGobj.GetComponent<CinemachineSmoothPath>();
                }

                if (path != null)
                { 
                    var ext = inst.mainVCamera.AddComponent<CinemachineDollyDirectionBaseCameraOffset>();
                    ext.m_Path = path;
                    ext.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
                    inst.mainVCamera.AddExtension(ext);
                }
            }
            //----ui相机会在游戏启动一瞬间消失了
            UISystem.instance?.UICamera?.gameObject.SetActive(false);
            UISystem.instance?.UICamera?.gameObject.SetActive(true);
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

