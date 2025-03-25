using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using LogicEntityType = PJR.EntityDefine.LogicEntityType;
using Object = UnityEngine.Object;

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

        public int EmployerLogicEntityID
        { 
            get { return employerLogicEntityID; }
            set { employerLogicEntityID = value; }
        }
        protected int employerLogicEntityID;

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
        [InlineButton("ChooseModelAsset", SdfIconType.Folder,"")]
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


        [NonSerialized]
        private Action<Object> _setter;

        void ChooseModelAsset()
        {
            EditorGUIUtility.ShowObjectPicker<Object>(null, false, "t:prefab", 0);
            _setter = (obj) =>
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                modelName = Path.GetFileName(assetPath);
            };
        }
        void ChooseAsset()
        {
            EditorGUIUtility.ShowObjectPicker<Object>(null, false, "t:prefab", 0);
            _setter = (obj) =>
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                animationClipSet = Path.GetFileName(assetPath);
            };
        }
        [OnInspectorGUI]
        void OnInspectorGUI()
        {
            if (Event.current != null && Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == "ObjectSelectorUpdated")
                {
                    var selected = EditorGUIUtility.GetObjectPickerObject();
                    if (_setter != null)
                    {
                        _setter.Invoke(selected);
                    }
                }
                else if (Event.current.commandName == "ObjectSelectorClosed")
                {
                    var selected = EditorGUIUtility.GetObjectPickerObject();
                    if (_setter != null)
                    {
                        _setter.Invoke(selected);
                        _setter = null;
                    }
                }
            }
        }
#endif
    }
}