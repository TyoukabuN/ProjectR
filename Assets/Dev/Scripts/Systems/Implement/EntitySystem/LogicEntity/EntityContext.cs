using Sirenix.OdinInspector;
using System;
using UnityEngine;
using LogicEntityType = PJR.EntityDefine.LogicEntityType;

namespace PJR
{
    public class EntityContext
    {
        public int LogicEntityID { 
            get { return logicEntityID; }
            set { 
                logicEntityID = value;
                logicEntityIDStr = logicEntityID.ToString(); 
            }
        }
        protected int logicEntityID = -1;
        public string LogicEntityIDStr { get => logicEntityIDStr; set => logicEntityIDStr = value; }

        private string logicEntityIDStr = string.Empty;

        public LogicEntityType entityType = LogicEntityType.Empty;

        //avatar相关
        public AvatarAssetNames avatarAssetNames;

        public Vector3 originPosition = Vector3.zero;
        public Vector3 originRotation = Vector3.zero;
        public Vector3 originScale = Vector3.one;


        public int jumpCount = 0;
        public void AddJumpCount()
        {
            jumpCount++;
        }
        public void RevertJumpCount()
        {
            jumpCount = 0;
        }

        private int triggeredCount = 0;
        public int TriggeredCount => triggeredCount;
        public void AddTriggeredCount() {  triggeredCount++; }
        public void RevertTriggeredCount() {  triggeredCount = 0; }
    }

    /// <summary>
    /// 构建Avatar所需的各种资源的assetPath
    /// </summary>
    [Serializable]
    public class AvatarAssetNames
    {
        public static AvatarAssetNames Empty = new AvatarAssetNames();
        //
        [LabelText("模型资源名字")]
        [ValidateInput("IsAssetNameValid_Prefab", "", InfoMessageType.Warning)]
        [InlineButton("ChooseAsset", SdfIconType.Folder,"")]
        public string modelName = string.Empty;
        [LabelText("动画集合名字")]
        [ValidateInput("IsAssetNameValid_Asset", "", InfoMessageType.Warning)]
        [InlineButton("ChooseAsset", SdfIconType.Folder, "")]
        public string animationClipSet = string.Empty;

#if UNITY_EDITOR
        public bool IsAssetNameValid_Prefab(string assetName, ref string error) => IsAssetNameValid(assetName, ".prefab", ref error);
        public bool IsAssetNameValid_Asset(string assetName, ref string error) => IsAssetNameValid(assetName, ".asset", ref error);
        public bool IsAssetNameValid(string assetName,string ext, ref string error)
        {
            error = "资源格式错误";
            if (string.IsNullOrEmpty(assetName))
            {
                error = "资源名字为空";
                return false;
            }
            if (assetName.IndexOf(ext) <= 0)
            {
                return false;
            }
            return true;
        }

        public void ChooseAsset(string searchFilter)
        {
            //UnityEngine.Object @object = null;
            //EditorGUIUtility.ShowObjectPicker<UnityEngine.Object>(@object, false, searchFilter, 0);
            //if(@object!= null)
            Debug.Log("还没有空弄...");
        }
#endif
    }
}