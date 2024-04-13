using UnityEngine;

namespace PJR
{
    public class EntityContext
    {
        public int logicEntityID = -1;

        public AvatarAssetNames avatarAssetNames;

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

    public class  AvatarAssetNames
    {
        public string modelName;
        public string animationClipSet;
    }
}