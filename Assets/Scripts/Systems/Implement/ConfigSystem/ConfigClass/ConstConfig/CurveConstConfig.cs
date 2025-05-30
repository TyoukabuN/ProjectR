using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class CurveConstConfig : ListConfigAsset<ConstConfigItem<AnimationCurve>>
    {

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/曲线常数配置")]
        public static void CreateConstConfigAsset()
        {
            PJR.Editor.CSConfigHelper.CreateListConfigAsset<CurveConstConfig, ConstConfigItem<AnimationCurve>>();
        }
#endif
    }
}
