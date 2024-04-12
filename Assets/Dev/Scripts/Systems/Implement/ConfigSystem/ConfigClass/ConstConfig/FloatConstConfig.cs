using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class FloatConstConfig : ListConfigAsset<ConstConfigItem<float>>
    {

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/Float常数配置")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateListConfigAsset<FloatConstConfig, ConstConfigItem<float>>();
        }
#endif
    }
}
