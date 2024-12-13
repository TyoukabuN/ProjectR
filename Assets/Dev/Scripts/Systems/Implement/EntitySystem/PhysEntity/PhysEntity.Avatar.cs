using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using static PJR.Systems.ResourceSystem;
using PJR.Systems;

namespace PJR
{
    public partial class PhysEntity : MonoBehaviour
    {
        [BoxGroup("Avatar"), ReadOnly] protected bool _avatarLoadDone = false;
        [BoxGroup("Avatar")] public bool AvatarLoadDone => _avatarLoadDone;

        [BoxGroup("Avatar"), ReadOnly] private Transform m_ModelRoot = null;
        [BoxGroup("Avatar"), ReadOnly] public Vector3 modelRoot_localPosition = Vector3.zero;
        [BoxGroup("Avatar"), ReadOnly] public Vector3 modelRoot_localEulerAngles = Vector3.zero;
        [BoxGroup("Avatar"), ReadOnly] public Vector3 modelRoot_localScale = Vector3.zero;
        public Transform ModelRoot
        {
            get
            {
                if (!m_ModelRoot)
                {
                    var gobj = new GameObject(EntityDefine.MODEL_ROOT_NAME);
                    gobj.transform.parent = this.transform;
                    m_ModelRoot = gobj.transform;
                    //
                    modelRoot_localPosition = m_ModelRoot.localPosition;
                    modelRoot_localEulerAngles = m_ModelRoot.localEulerAngles;
                    modelRoot_localScale = m_ModelRoot.localScale;
                }
                return m_ModelRoot;
            }
        }

        #region reference

        [BoxGroup("Avatar"), ReadOnly] public GameObject avatar;
        [BoxGroup("Avatar"), ReadOnly] public Transform avatarTran => avatar?.transform;

        #endregion

        #region callback

        public Action<PhysEntity> onAvatarLoadDone;
        public Action<PhysEntity> onClipSetLoadDone;

        #endregion


        #region AssetNams
        [BoxGroup("Avatar")] private AvatarAssetNames _assetNames = AvatarAssetNames.Empty;
        /// <summary>
        /// 模型所需的asset的名字
        /// </summary>
        public AvatarAssetNames AssetNames { get => _assetNames; set => _assetNames = value; }

        public string ModelName => AssetNames.modelName;
        public string AnimationClipSetName
        {
            get { 
                string assetName = AssetNames.animationClipSet;
                if(!string.IsNullOrEmpty(assetName))
                    return assetName;
                if (!string.IsNullOrEmpty(ModelName) && ModelName.IndexOf("Avatar_") == 0)
                {
                    string avatarName = ModelName.Replace("Avatar_", "").Replace(".prefab", "");
                    if (string.IsNullOrEmpty(avatarName))
                        return string.Empty;
                    AssetNames.animationClipSet = string.Format(EntityDefine.AnimationSetFormat, avatarName);
                    return AssetNames.animationClipSet;
                }
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// 根据avatar所需的assetNames加载/创建avatar
        /// </summary>
        /// <param name="assetNames"></param>
        public void CreateAvatar(AvatarAssetNames assetNames)
        {
            if (assetNames == null)
                return;
            this._assetNames = assetNames;

            BeginLoadAsset();
        }

        /// <summary>
        /// 根据avatar所需的assetNames加载/创建avatar
        /// </summary>
        /// <param name="assetNames"></param>
        public void CreateAvatar(LogicEntity logicEntity, PhysEntityComponentRequire physRequire, Action<PhysEntity> onAvatarLoadDone, Action onDrawGizmos)
        {
            if (logicEntity == null)
                return;
            this.logicEntity = logicEntity;
            this._assetNames = logicEntity.entityContext.avatarAssetNames;

            this.physRequire = physRequire;
            if(onAvatarLoadDone != null) this.onAvatarLoadDone += onAvatarLoadDone;
            if(onDrawGizmos != null) this.onDrawGizmos += onDrawGizmos;

            gameObject.name = $"{logicEntity.entityName}_{physEntityId}";

            BeginLoadAsset();
        }
        public void CreateAvatar(LogicEntity logicEntity, Action<PhysEntity> onAvatarLoadDone, Action onDrawGizmos) => CreateAvatar(logicEntity, PhysEntityComponentRequire.Default, onAvatarLoadDone, onDrawGizmos);
        public void CreateAvatar(LogicEntity logicEntity, PhysEntityComponentRequire physRequire, Action onDrawGizmos) => CreateAvatar(logicEntity, physRequire, null, onDrawGizmos);
        public void CreateAvatar(LogicEntity logicEntity, PhysEntityComponentRequire physRequire, Action<PhysEntity> onAvatarLoadDone) => CreateAvatar(logicEntity, physRequire, onAvatarLoadDone, null);
        public void CreateAvatar(LogicEntity logicEntity, PhysEntityComponentRequire physRequire) => CreateAvatar(logicEntity, physRequire, null, null);
        public void CreateAvatar(LogicEntity logicEntity) => CreateAvatar(logicEntity, PhysEntityComponentRequire.Default, null, null);


        public void SetTransformByEntityContext(EntityContext context)
        {
            transform.position = context.originPosition;
            transform.eulerAngles = context.originRotation;
            transform.localScale = context.originScale;
        }

        void BeginLoadAsset()
        {
            _avatarLoadDone = false;
            StartCoroutine(LoadAssets());
        }

        /// <summary>
        /// 加载协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadAssets()
        {
            //TODO:弄成单独部位的加载器
            //模型
            if (!string.IsNullOrEmpty(ModelName))
            { 
                var loader_avatar = ResourceSystem.LoadAsset<GameObject>(_assetNames.modelName);
                if (loader_avatar == null)
                {
                    LogSystem.LogError($"[{nameof(LoadAssets)}] Failure to load avatar asset");
                    yield return null;
                }
                yield return loader_avatar;
                OnLoadAvatarLoadDone(loader_avatar);
            }

            if (!string.IsNullOrEmpty(AnimationClipSetName))
            {
                //动画集
                var loader_clipSet = ResourceSystem.LoadAsset<AnimatiomClipTransitionSet>(_assetNames.animationClipSet);
                if (loader_clipSet == null)
                {
                    LogSystem.LogError($"[{nameof(LoadAssets)}] Failure to load {nameof(AnimatiomClipTransitionSet)} asset");
                    yield return null;
                }
                yield return loader_clipSet;
                OnLoadClipSetDone(loader_clipSet);
            }

            OnLoadAllLoadDone();
        }
        
        /// <summary>
        /// 加载模型完成
        /// </summary>
        /// <param name="loader"></param>
        protected void OnLoadAvatarLoadDone(ResourceLoader loader)
        {
            var asset = loader.GetRawAsset<GameObject>();
            if (asset == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadAvatarLoadDone)}] Failure to load avatar asset");
                return;
            }

            avatar = GameObject.Instantiate(asset);
            if (avatar == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadAvatarLoadDone)}] Failure to instantiate avatar");
                return;
            }

            InitAvatar(avatar);
        }
        /// <summary>
        /// 动画集加载完成
        /// </summary>
        /// <param name="loader"></param>
        protected void OnLoadClipSetDone(ResourceLoader loader)
        {
            var asset = loader.GetRawAsset<AnimatiomClipTransitionSet>();
            if (asset == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadAvatarLoadDone)}] Failure to load avatar asset");
                return;
            }
            animationClipTransitionSet = asset;
            Init_Animation_ClipSet(asset);
        }
        /// <summary>
        /// 全部加载完成
        /// </summary>
        protected void OnLoadAllLoadDone()
        {
            _avatarLoadDone = true;

            if (onAvatarLoadDone != null)
                onAvatarLoadDone.Invoke(this);
        }

        /// <summary>
        /// 初始化模型
        /// </summary>
        /// <param name="avatar"></param>
        protected void InitAvatar(GameObject avatar)
        {
            avatar.transform.SetParent(ModelRoot, false);
            avatar.transform.ResetValue();

            SetTransformByEntityContext(logicEntity.entityContext);

            Init_Collection(avatar);
            Init_Animation_Reference(avatar);
            Init_KCC(avatar);
        }
    }
}
