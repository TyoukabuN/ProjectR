using UnityEngine;

#if UNITY_EDITOR
using PJR.Editor;
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
            CSConfigHelper.CreateListConfigAsset<CurveConstConfig, ConstConfigItem<AnimationCurve>>();
        }
#endif
    }
}
