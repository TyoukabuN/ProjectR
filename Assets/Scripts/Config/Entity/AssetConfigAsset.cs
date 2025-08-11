using System;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public class AssetConfigAsset : ScriptableObject
    {
        public AvatarAssetNames avatarAssetNames = new AvatarAssetNames();
    }
}
