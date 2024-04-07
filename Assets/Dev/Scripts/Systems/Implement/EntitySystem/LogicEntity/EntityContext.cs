using UnityEngine;

namespace PJR
{
    public class EntityContext
    {
        public int logicEntityID = -1;

        public AvatarAssetNames avatarAssetNames;

        public Vector2 inputAxi;
        public Vector2 mouseDelta;
        public int runValue;
        public int grounded = 1;
        struct State { }
    }

    public class  AvatarAssetNames
    {
        public string modelName;
        public string animationClipSet;
    }
}