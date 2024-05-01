using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PJR
{
    public partial class SceneSystem : MonoSingletonSystem<SceneSystem>
    {
        /// <summary>
        /// 生成场景中的实体
        /// </summary>
        /// <param name="sceneRoot"></param>
        public void GenSceneEntity(GameObject sceneRoot = null)
        {
            sceneRoot = sceneRoot == null ? instance.sceneRoot : sceneRoot;
            if (sceneRoot == null)
                return;

            //Player
            if (EntitySystem.LocalPlayer == null)
            {
                var entityContext = new EntityContext();
                entityContext.originPosition = PlayBornPoint.transform.position;
                EntitySystem.CreatePlayer(entityContext);
            }
            //Trap
            SceneTrapRoot = sceneRoot.transform.Find(EntityDefine.SCENE_ROOT_NAME_TRAP);
            var trapHosts = SceneTrapRoot.GetComponentsInChildren<TrapConfigHost>();
            foreach (var host in trapHosts)
            {
                if (host.configAsset == null)
                    continue;
                EntitySystem.CreateTrapEntity(host);
            }
        }
    }
}