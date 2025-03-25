using UnityEngine;

namespace PJR.Config
{
    public class AvatarConfigItemAsset : OrdinalConfigItemAssetTemplate
    {
        [SerializeField]
        private AvatarAssetNames _avatarAssetName;
        public AvatarAssetNames AvatarAssetName => _avatarAssetName;
    }
}
