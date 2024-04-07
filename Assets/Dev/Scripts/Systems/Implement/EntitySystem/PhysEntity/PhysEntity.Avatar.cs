using Animancer;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

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
                    var gobj = new GameObject(TEntityDefine.MODEL_ROOT_NAME);
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

        [BoxGroup("Avatar")] private AvatarAssetNames _assetNames;
        /// <summary>
        /// 模型所需的asset的名字
        /// </summary>
        public AvatarAssetNames AssetNames { get => _assetNames; set => _assetNames = value; }

        /// <summary>
        /// 根据avatar所需的assetNames加载/创建avatar
        /// </summary>
        /// <param name="assetNames"></param>
        public void CreateAvatar(AvatarAssetNames assetNames)
        {
            if (assetNames == null)
                return;
            this._assetNames = assetNames;
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
            var loader_avatar = ResourceSystem.LoadAsset<GameObject>(_assetNames.modelName);
            if (loader_avatar == null)
            {
                LogSystem.LogError($"[{nameof(LoadAssets)}] Failure to load avatar asset");
                yield return null;
            }
            yield return loader_avatar;
            OnLoadAvatarLoadDone(loader_avatar);

            //动画集
            var loader_clipSet = ResourceSystem.LoadAsset<AnimatiomClipTransitionSet>(_assetNames.animationClipSet);
            if (loader_clipSet == null)
            {
                LogSystem.LogError($"[{nameof(LoadAssets)}] Failure to load {nameof(AnimatiomClipTransitionSet)} asset");
                yield return null;
            }
            yield return loader_clipSet; 
            OnLoadClipSetDone(loader_clipSet);

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
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.rotation = Quaternion.identity;

            Init_Collection(avatar);
            Init_Animation_Reference(avatar);
            Init_KCC(avatar);
        }
    }
}
