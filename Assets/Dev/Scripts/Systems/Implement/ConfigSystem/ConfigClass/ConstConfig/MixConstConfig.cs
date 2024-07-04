using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class MixConstConfig : TableListConfigAsset<MixConstConfigItem>
    {

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/混合常数配置")]
        public static void CreateConstConfigAsset()
        {
            PJR.Editor.CSConfigHelper.CreateTableListConfigAsset<MixConstConfig, MixConstConfigItem>();
        }
#endif
    }
}
