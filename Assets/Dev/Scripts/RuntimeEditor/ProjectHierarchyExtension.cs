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
        static ProjectHierarchyExtension()
        {
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }
        /// <summary>
        /// 文件配置转中文描述
        /// </summary>
        static Dictionary<string, string> type2desc = new Dictionary<string, string>() {
            { typeof(EntityPhysicsConfigAsset).FullName,"实体物理数值配置"},
            { typeof(MixConstConfig).FullName,"混合常数配置"},
            { typeof(IntConstConfig).FullName,"Int常数配置"},
            { typeof(FloatConstConfig).FullName,"Float常数配置"},
            { typeof(CurveConstConfig).FullName,"曲线常数配置"},
            { typeof(EntityAttributeConfigAsset).FullName,"实体属性配置"},
        };


        /// <summary>
        /// 文件路径
        /// </summary>
        static Dictionary<string, GUIContent> folderPath2desc = new Dictionary<string, GUIContent>()
        {
            {"Assets/Art",new GUIContent("美术相关")},

            {"Assets/Art/Audio",new GUIContent("声音")},
            {"Assets/Art/Audio/Oggs",new GUIContent("声音ogg文件")},
            {"Assets/Art/Audio/Prefabs",new GUIContent("声音场景放置预置")},

            {"Assets/Art/Avatar",new GUIContent("模型")},
            {"Assets/Art/Avatar/Player",new GUIContent("玩家角色")},
            {"Assets/Art/Avatar/Trap",new GUIContent("机关陷阱")},

            {"Assets/Art/Effect",new GUIContent("特效")},

            {"Assets/Art/Env",new GUIContent("场景相关")},
            {"Assets/Art/Env/Scene",new GUIContent("场景")},

            //
            {"Assets/Dev",new GUIContent("程序开发相关")},
            {"Assets/Dev/Lab",new GUIContent("做实验都放这")},
            {"Assets/Dev/Prefabs",new GUIContent("程序相关资源(如配置)")},
            {"Assets/Dev/Scripts",new GUIContent("代码相关")},
            {"Assets/Dev/Scripts/Camera",new GUIContent("摄像机相关")},
            {"Assets/Dev/Scripts/Config",new GUIContent("配置相关")},
            {"Assets/Dev/Scripts/Config/Entity",new GUIContent("实体配置")},
            {"Assets/Dev/Scripts/Config/Resource",new GUIContent("资源配置")},
            {"Assets/Dev/Scripts/Config/ConfigWrap",new GUIContent("配置Wrap")},
            {"Assets/Dev/Scripts/Config/ConfigWrap/Gen",new GUIContent("程序生成配置Wrap")},
            {"Assets/Dev/Scripts/Core",new GUIContent("关键/通用")},
            {"Assets/Dev/Scripts/Core/NumericalControl",new GUIContent("数值控制")},
            {"Assets/Dev/Scripts/Gen",new GUIContent("程序生成代码")},
            {"Assets/Dev/Scripts/Manager",new GUIContent("管理器相关")},
            {"Assets/Dev/Scripts/Modules",new GUIContent("模块(对Systems下基类的实现)")},
            {"Assets/Dev/Scripts/Profiler",new GUIContent("性能分析相关")},
            {"Assets/Dev/Scripts/RuntimeEditor",new GUIContent("运行时Editor")},
            {"Assets/Dev/Scripts/StateMachine",new GUIContent("状态机相关")},
            {"Assets/Dev/Scripts/StateMachine/ScriptState",new GUIContent("纯代码状态机")},
            {"Assets/Dev/Scripts/Systems",new GUIContent("系统相关")},
            {"Assets/Dev/Scripts/Systems/Implement",new GUIContent("系统实现")},
            {"Assets/Dev/Scripts/Systems/Implement/ConfigSystem",new GUIContent("配置系统")},
            {"Assets/Dev/Scripts/Systems/Implement/ConfigSystem/ConfigClass",new GUIContent("配置类实现")},
            {"Assets/Dev/Scripts/Systems/Implement/EntitySystem",new GUIContent("实体系统")},
            {"Assets/Dev/Scripts/Systems/Implement/EntitySystem/LogicEntity",new GUIContent("逻辑实体")},
            {"Assets/Dev/Scripts/Systems/Implement/EntitySystem/PhysEntity",new GUIContent("物理实体")},
            {"Assets/Dev/Scripts/Systems/Implement/ForceFieldSystem",new GUIContent("力场系统")},
            {"Assets/Dev/Scripts/Systems/Implement/InputSystem",new GUIContent("输入系统")},
            {"Assets/Dev/Scripts/Systems/Implement/LogSystem",new GUIContent("日志系统")},
            {"Assets/Dev/Scripts/Systems/Implement/ResourceSystem",new GUIContent("资源系统")},
            {"Assets/Dev/Scripts/Systems/Implement/SceneSystem",new GUIContent("场景系统")},
            {"Assets/Dev/Scripts/Utility",new GUIContent("通用工具")},
            {"Assets/Dev/Scripts/Utility/ClassExtension",new GUIContent("扩展类")},
        };
    }
}
#endif
