using UnityEngine;

namespace PJR.Systems
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
                entityContext.avatarAssetNames = new AvatarAssetNames() {
                    modelName = "Avatar_Slime.prefab",
                };
                EntitySystem.CreatePlayer(entityContext);
            }
            //Monster
            if (envSetting?.GenSceneMonster ?? false)
                if (SceneEnemyRoot != null)
                {
                    var hosts = SceneEnemyRoot.GetComponentsInChildren<MonsterConfigHost>();
                    foreach (var host in hosts)
                    {
                        if (!host.LoadOnSceneEnter)
                            continue;
                        EntitySystem.CreateMonster(host);
                    }
                }
            //Trap
            if (envSetting?.GenSceneTrap ?? false)
                if (SceneTrapRoot != null)
                {
                    var hosts = SceneTrapRoot.GetComponentsInChildren<TrapConfigHost>();
                    foreach (var host in hosts)
                    {
                        if (host.configAsset == null)
                            continue;
                        if (!host.LoadOnSceneEnter)
                            continue;
                        EntitySystem.CreateTrapEntity(host);
                    }
                }
            //Item
            if (envSetting?.GenSceneItem ?? false)
                if (SceneItemRoot != null)
                {
                    var hosts = SceneItemRoot.GetComponentsInChildren<ItemConfigHost>();
                    foreach (var host in hosts)
                    {
                        if (host.configAsset == null)
                            continue;
                        EntitySystem.CreateItemEntity(host);
                    }
                }
        }
    }
}