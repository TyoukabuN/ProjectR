using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public class AssetConfigAsset : ScriptableObject
    {
        public AvatarAssetNames avatarAssetNames = new AvatarAssetNames();
    }
}
