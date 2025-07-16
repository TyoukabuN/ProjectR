#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class EditorPreferences
    {
        private static string EditorPrefKey_TimeReferenceMode = "PJR.Timeline.Editor.EditorPreferences.TimeReferenceMode";
        private static string EditorPrefKey_TimeFormat = "PJR.Timeline.Editor.EditorPreferences.TimeFormat";
        
        public static EnumPref<TimeReferenceMode> TimeReferenceMode = new(EditorPrefKey_TimeReferenceMode);
        public static EnumPref<TimeFormat> TimeFormat = new(EditorPrefKey_TimeFormat);

        
        public struct EnumPref<T> where T : Enum
        {
            private bool _hasValue;
            private T _cache;
            private readonly string _editorPrefKey;

            public T Value
            {
                get
                {
                    if (!_hasValue)
                    {
                        _cache = (T)Enum.ToObject(typeof(T), EditorPrefs.GetInt(_editorPrefKey, 0));
                        _hasValue = true;
                    }
                    return _cache;
                }
                set
                {
                    _cache = value;
                    EditorPrefs.SetInt(_editorPrefKey,Convert.ToInt32(value));
                }
            }

            public EnumPref(string editorPrefKey)
            {
                _cache = default;
                _hasValue = false;
                _editorPrefKey = editorPrefKey;
            }
        }
    }
}
