using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PJR
{
    public partial class SceneSystem : MonoSingletonSystem<SceneSystem>
    {
        public static event UnityAction<Scene, LoadSceneMode> onSceneLoaded;
        public static event UnityAction<Scene> onSceneUnloaded;
        public static event UnityAction<Scene, Scene> onActiveSceneChanged;

        /// <summary>
        /// register record for unregister 
        /// </summary>
        private List<UnityAction<Scene, LoadSceneMode>> _onSceneLoadedCallbacks = new List<UnityAction<Scene, LoadSceneMode>>();
        //TODO: 清理逻辑
        private List<UnityAction<Scene>> _onSceneUnloadedCallbacks = new List<UnityAction<Scene>>();
        private List<UnityAction<Scene, Scene>> _onActiveSceneChangedCallbacks = new List<UnityAction<Scene, Scene>>();

        public GameObject sceneRoot = null;
        public EnvSetting envSetting = null;
        //Roots
        public Transform SceneTrapRoot = null;
        public Transform PlayBornPoint = null;
        public Transform SceneEnemyRoot = null;
        public Transform SceneItemRoot = null;
        public Transform SceneResetPointRoot = null;
        public Transform SceneDeadZoneRoot = null;
        //
        public List<Transform> ResetPoints = null;


        public override IEnumerator Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            yield return null;
        }

        /// <summary>
        /// 检查不是已经在一个合格游戏场景中
        /// </summary>
        /// <returns></returns>
        public static bool CheckReadyInValidGameScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene == null)
                return false; ;
            foreach (var gobj in currentScene.GetRootGameObjects())
            {
                if (gobj.tag == "SceneRoot")
                {
                    instance.sceneRoot = gobj;
                    instance.envSetting = gobj.GetComponent<EnvSetting>();
                    break;
                }
            }

            if (instance.sceneRoot == null)
                return false;

            instance.PlayBornPoint = instance.sceneRoot.transform.Find("Procedure/PlayBornPoint");
            instance.OnActiveSceneChanged(currentScene, currentScene);
            return true;
        }

        /// <summary>
        /// 还没有写场景加载,不知道放哪里待用好
        /// </summary>
        /// <param name="sceneRoot"></param>
        public void OnEnterScene(GameObject sceneRoot = null)
        {
            sceneRoot = sceneRoot == null ? instance.sceneRoot : sceneRoot;
            SceneEnemyRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_ENEMY);
            SceneTrapRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_TRAP);
            SceneItemRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_ITEM);
            SceneResetPointRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_RESET_POINT);
            if (SceneResetPointRoot != null)
            {
                ResetPoints = new List<Transform>();
                for (int i = 0; i < SceneResetPointRoot.childCount; i++)
                { 
                    var point = SceneResetPointRoot.GetChild(i);
                    ResetPoints.Add(point);
                }
            }
            SceneDeadZoneRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_DEAdZONE);

            GenSceneEntity();
        }

        public bool GetClosestResetPoint(Vector3 refPoint,out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            if (ResetPoints == null)
            { 
                LogSystem.LogError("场景没有设置重置点");
                return false;
            }
            float closestDis = float.MaxValue;
            for (int i = 0; i < ResetPoints.Count; i++)
            {
                var resetPoint = ResetPoints[i].position;
                float dis = (refPoint - resetPoint).magnitude;
                if (dis <= closestDis)
                {
                    closestPoint = resetPoint;
                    closestDis = dis;
                }
            }
            return true;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            LogSystem.Log("SceneSystem", $"[OnSceneLoaded] {scene.name} {loadSceneMode}");
            _onSceneLoadedCallbacks?.ForEach(action => action?.Invoke(scene, loadSceneMode));
        }
        public void OnSceneUnloaded(Scene scene)
        {
            LogSystem.Log("SceneSystem", $"[OnSceneUnloaded] {scene.name} ");
            _onSceneUnloadedCallbacks?.ForEach(action => action?.Invoke(scene));
        }
        public void OnActiveSceneChanged(Scene sceneA, Scene sceneB)
        {
            LogSystem.Log("SceneSystem", $"[OnActiveSceneChanged] {sceneA.name}  {sceneB.name} ");
            _onActiveSceneChangedCallbacks?.ForEach(action => action?.Invoke(sceneA, sceneB));
        }

        public static void SetSceneLoaded(UnityAction<Scene, LoadSceneMode> callback)
        {
            instance._onSceneLoadedCallbacks.Add(callback);
        }
        public static void SetSceneUnloaded(UnityAction<Scene> callback)
        {
            instance._onSceneUnloadedCallbacks.Add(callback);
        }
        public static void SetActiveSceneChanged(UnityAction<Scene, Scene> callback)
        {
            instance._onActiveSceneChangedCallbacks.Add(callback);
        }

        public override void Clear()
        {
            _onSceneLoadedCallbacks?.ForEach(action => SceneManager.sceneLoaded -= action);
            _onSceneUnloadedCallbacks?.ForEach(action => SceneManager.sceneUnloaded -= action);
            _onActiveSceneChangedCallbacks?.ForEach(action => SceneManager.activeSceneChanged -= action);
        }
    }
}