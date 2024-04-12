using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class IntConstConfig : ListConfigAsset<IntConstConfigItem>
    {

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/Int常数配置")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateListConfigAsset<IntConstConfig, IntConstConfigItem>();
        }
#endif
    }
}
