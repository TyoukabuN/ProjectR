#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PJR
{
    public partial class ProjectHierarchyExtension : MonoBehaviour
    {
        /// <summary>
        /// 文件配置转中文描述
        /// </summary>
        static Dictionary<string, string> type2desc = new Dictionary<string, string>() {
            { typeof(EntityPhysicsConfigAsset).FullName,"实体物理数值配置"},
            { typeof(MixConstConfig).FullName,"混合常数配置"},
            { typeof(IntConstConfig).FullName,"Int常数配置"},
            { typeof(FloatConstConfig).FullName,"Float常数配置"},
            { typeof(CurveConstConfig).FullName,"曲线常数配置"},
        };
    }
}
#endif
