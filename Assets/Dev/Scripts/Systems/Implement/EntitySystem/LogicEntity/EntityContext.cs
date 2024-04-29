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
        public Vector3 originScale = Vector3.zero;

        public int jumpCount = 0;
        public void AddJumpCount()
        {
            jumpCount++;
        }
        public void RevertJumpCount()
        {
            jumpCount = 0;
        }
    }

    /// <summary>
    /// 构建Avatar所需的各种资源的assetPath
    /// </summary>
    public class  AvatarAssetNames
    {
        public string modelName;
        public string animationClipSet;
    }
}