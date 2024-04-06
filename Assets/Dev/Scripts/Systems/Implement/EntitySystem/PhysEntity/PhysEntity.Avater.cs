using Animancer;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace PJR
{
    public partial class PhysEntity : MonoBehaviour
    {
        private Transform m_ModelRoot = null;
        [HideInInspector] public Vector3 modelRoot_localPosition = Vector3.zero;
        [HideInInspector] public Vector3 modelRoot_localEulerAngles = Vector3.zero;
        [HideInInspector] public Vector3 modelRoot_localScale = Vector3.zero;
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

#region callback
        public Action<PhysEntity> onAvaterLoadDone;
#endregion

        public void CreateAvater(string assetFullName)
        {
            var loader = ResourceSystem.LoadAsset<GameObject>(assetFullName);
            loader.OnDone += OnCreateAvaterLoadDone_Internal;
        }

        public GameObject avater;
        public Transform avaterTran => avater?.transform;
        protected void OnCreateAvaterLoadDone_Internal(ResourceLoader loader)
        {
            var asset = loader.GetAsset<GameObject>();
            if (asset == null)
            {
                LogSystem.LogError($"[{nameof(OnCreateAvaterLoadDone_Internal)}] Failure to load avater asset");
                return;
            }

            avater = GameObject.Instantiate(asset);
            if (avater == null)
            {
                LogSystem.LogError($"[{nameof(OnCreateAvaterLoadDone_Internal)}] Failure to instantiate avater");
                return;
            }

            InitAvater(avater);

            if (onAvaterLoadDone != null) 
                onAvaterLoadDone.Invoke(this);
        }

        protected void InitAvater(GameObject avater)
        {
            avater.transform.SetParent(ModelRoot, false);
            avater.transform.localPosition = Vector3.zero;
            avater.transform.rotation = Quaternion.identity;

            Init_Collection(avater);
            Init_Animation(avater);
        }
    }
}
