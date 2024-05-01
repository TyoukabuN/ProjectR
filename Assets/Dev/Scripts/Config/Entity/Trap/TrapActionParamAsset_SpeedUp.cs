using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    [InlineEditor]
    public class TrapActionParamAsset_SpeedUp : TrapActionParamAsset
    {
        [LabelText("速度系数")] public float speed = 2f;
        [LabelText("持续时间")] public float duration = 3f;
        [LabelText("衰减系数")][PropertyTooltip(ExtraVelocity.tooltip)] public float damp = -1f;
        [InlineButton("ShowEasingHelpUrl","曲线示例")]
        [LabelText("衰减曲线")] public Easing.Ease easing = Easing.Ease.Linear;
        [LabelText("强制移动阻断输入")] public bool blockInput = false;

#if UNITY_EDITOR
        public static void ShowEasingHelpUrl()
        {
            Application.OpenURL("https://easings.net/");
        }
        [MenuItem("Assets/PJR/创建配置/机关陷阱/参数Asset/速度提升参数")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateScriptableObject<TrapActionParamAsset_SpeedUp>();
        }
#endif
    }
}
