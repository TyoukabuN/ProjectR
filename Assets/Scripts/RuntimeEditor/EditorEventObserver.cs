#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PJR.Config;
using Sirenix.Utilities;
using UnityEditor;

namespace PJR.Editor
{
    /// <summary>
    /// 将一些Editor事件的注册都放在一个地方
    /// </summary>
    public static class EditorEventObserver
    {
        public static event Action<PlayModeStateChange> PlayModeStateChanged;

        private static List<PlayModeStateChangeRequire> _playModeStateChangeRequires;

        [InitializeOnLoadMethod]
        static void RegisterEvent()
        {
            //处理PlayMode变化的事件
            EditorApplication.playModeStateChanged -= Internal_OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += Internal_OnPlayModeStateChanged;

            _playModeStateChangeRequires ??= new List<PlayModeStateChangeRequire>();
            var menuItemTypes = 
                TypeCache.GetMethodsWithAttribute<ExecuteWhenPlayModeStateChangeAttribute>()
                    .Where(x => !x.IsAbstract && x.IsStatic);
            foreach (var methodInfo in menuItemTypes)
            {
                _playModeStateChangeRequires.Add(new PlayModeStateChangeRequire(methodInfo, methodInfo.GetAttribute<ExecuteWhenPlayModeStateChangeAttribute>()));
            }
        }
        static void Internal_OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PlayModeStateChanged?.Invoke(state);

            if (_playModeStateChangeRequires != null)
            {
                for (var i = 0; i < _playModeStateChangeRequires.Count; i++)
                {
                    _playModeStateChangeRequires[i]?.TryInvoleMethod(state);
                }
            }
        }
    }

    class PlayModeStateChangeRequire
    {
        private MethodInfo _methodInfo;
        private ExecuteWhenPlayModeStateChangeAttribute _attribute;
        
        public PlayModeStateChangeRequire(MethodInfo methodInfo, ExecuteWhenPlayModeStateChangeAttribute requireAttribute)
        {
            _methodInfo = methodInfo;
            _attribute = requireAttribute;
        }
        
        public void TryInvoleMethod(PlayModeStateChange state)
        {
            if (_attribute == null)
                return;
            if (_attribute.EPlayModeStateChange == state)
                return;
            _methodInfo?.Invoke(null, null);
        }
    }

    /// <summary>
    /// 当触发某个playModeStateChanged时条用
    /// 当playModeStateChanged的时候需要release或者clear的系统可能会用到
    /// </summary>
    public class ExecuteWhenPlayModeStateChangeAttribute : Attribute
    {
        public PlayModeStateChange EPlayModeStateChange;

        public ExecuteWhenPlayModeStateChangeAttribute(PlayModeStateChange playModeStateChange)
        {
            EPlayModeStateChange = playModeStateChange;
        }
    }
}

#endif
