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
    public class TrapActionParamAsset_AddForce : TrapActionParamAsset
    {
        [LabelText("力")] public Vector3 force = Vector3.zero;
        [LabelText("持续时间")] public float duration = 0.333f;
        [LabelText("衰减系数")][PropertyTooltip(ExtraVelocity.tooltip)] public float damp = -1f;
        [InlineButton("ShowEasingHelpUrl","曲线示例")]
        [LabelText("衰减曲线")] public Easing.Ease easing = Easing.Ease.Linear;

#if UNITY_EDITOR
        public static void ShowEasingHelpUrl()
        {
            Application.OpenURL("https://easings.net/");
        }
        [MenuItem("Assets/PJR/创建配置/机关陷阱/参数Asset/施力参数")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateScriptableObject<TrapActionParamAsset_AddForce>();
        }
#endif
    }
}
