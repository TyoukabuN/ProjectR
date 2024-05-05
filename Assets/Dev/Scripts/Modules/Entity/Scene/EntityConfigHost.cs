using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class EntityConfigHost : MonoBehaviour
    {
        public abstract EntityConfigAsset EntiyConfigAsset { get; }

        public AvatarAssetNames assetName = new AvatarAssetNames();

        public bool LoadOnSceneEnter = true;

        protected virtual void Awake()
        {
            if (EntiyConfigAsset != null)
            {
                EntiyConfigAsset.host = this;
            }
        }

        public virtual void GenrateEntity()
        {
            if (EntiyConfigAsset == null)
            {
                LogSystem.LogError("[EntityConfigHost.GenrateEntity] EntiyConfigAsset == null");
                return;
            }
            EntiyConfigAsset.GenrateEntity();
        }
    }
}
