using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PJR
{
    public partial class ResourceSystem
    {
        public const string GuidPattern = @"(?<guid>^[\w]+)?(\[(?<subAsset>[^\[\]]+)\]$)?";
        /// <summary>
        /// 传进来的guid可能这样的：
        /// bf2889ed61cf07241a607a6deed04f55[153360_ECFG_Humanoid_GoblinArcher_10003]
        /// 来自<see cref="AssetReference.RuntimeKey"/>
        /// 或来自<see cref="AddressableExtensions.GetAddress"/>
        /// </summary>
        /// <param name="assetGUID"></param>
        /// <param name="guid"></param>
        /// <param name="subAssetName"></param>
        public static void GetGUIDInfo(string assetGUID, out string guid, out string subAssetName)
        {
            guid = assetGUID;
            subAssetName = null;
            Match match = new Regex(GuidPattern).Match(assetGUID);
            if (match.Success)
            {
                guid = match.Groups["guid"].Value;
                subAssetName = match.Groups["subAsset"].Value;
            }
        }
    }
}