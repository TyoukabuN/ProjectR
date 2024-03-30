//-----------------------------------------------------------------------
// <copyright file="ProjectSetting.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Internal
{
#pragma warning disable

    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Globalization;
    using UnityEditor;
    using UnityEngine;

    public interface IProjectSetting
    {
        void Reset();
        void SetInitData(string key, object defaultValue, UnityEngine.Object serializedContainer);
    }

    [Serializable, InlineProperty]
    public abstract class ProjectSetting<T> : IProjectSetting
    {
        [SerializeField, HideInInspector]
        private T serializedValue;

        [SerializeField, HideInInspector]
        private bool changedFromDefault;

        [NonSerialized]
        private string key;

        [NonSerialized]
        private bool localOverride;

        [NonSerialized]
        private T currentValue;

        [NonSerialized]
        private bool initialized;

        [NonSerialized]
        private bool hasInitData;

        [NonSerialized]
        private T defaultValue;

        [NonSerialized]
        private UnityEngine.Object serializedContainer;

        [NonSerialized]
        private static Func<T, T, bool> comparer;

        [ShowInInspector, HorizontalGroup(20), HideLabel]
        public bool LocalOverride
        {
            get
            {
                this.EnsureInitialized();
                return this.localOverride;
            }

            set
            {
                this.EnsureInitialized();

                if (this.localOverride != value)
                {
                    this.localOverride = value;
                    EditorPrefs.SetBool(this.key + "_OVERRIDE", value);

                    if (this.localOverride)
                    {
                        this.currentValue = this.GetLocalValue(this.key, this.defaultValue);
                    }
                    else
                    {
                        this.currentValue = this.serializedValue;
                    }
                }
            }
        }

        public string Key { get { return this.key; } }

        [HorizontalGroup, HideLabel, SuppressInvalidAttributeError, InlineProperty]
        [ShowInInspector]
        public T Value
        {
            get
            {
                this.EnsureInitialized();
                return this.currentValue;
            }
            set
            {
                this.EnsureInitialized();
                if (!this.Equals(this.currentValue, value))
                {
                    this.currentValue = value;

                    if (this.localOverride)
                    {
                        this.SetLocalValue(this.Key, value);
                    }
                    else
                    {
                        this.changedFromDefault = true;
                        this.serializedValue = value;
                    }
                }
            }
        }

        public ProjectSetting() { }

        public ProjectSetting(string key, T defaultValue, UnityEngine.Object serializedContainer)
        {
            (this as IProjectSetting).SetInitData(key, defaultValue, serializedContainer);
        }

        void IProjectSetting.SetInitData(string key, object defaultValue, UnityEngine.Object serializedContainer)
        {
            this.key = key;
            this.defaultValue = (T)defaultValue;
            this.hasInitData = true;
            this.serializedContainer = serializedContainer;
        }

        public void Reset()
        {
            this.ThrowIfNoInitData();
            this.DeleteLocalValue();
            this.LocalOverride = false;
            this.currentValue = this.defaultValue;
            this.serializedValue = default(T);
            this.changedFromDefault = false;
        }

        public void DeleteLocalValue()
        {
            this.ThrowIfNoInitData();
            EditorPrefs.DeleteKey(this.key);
            this.initialized = false;
        }

        private void ThrowIfNoInitData()
        {
            if (!this.hasInitData)
            {
                throw new InvalidOperationException("A project setting of type '" + this.GetType().GetNiceName() + "' was used before init data was set. Did you forget to declare it in a class derived from ProjectSettingsConfig<T>, or did you forget to decorate its field with a [ProjectSettingKey(\"SOME_KEY\", some_default_value)]?");
            }
        }

        private void EnsureInitialized()
        {
            if (!this.initialized)
            {
                this.ThrowIfNoInitData();

                this.localOverride = EditorPrefs.GetBool(key + "_OVERRIDE", false);

                if (this.localOverride)
                {
                    this.currentValue = GetLocalValue(key, default(T));
                }
                else
                {
                    this.currentValue = this.changedFromDefault ? this.serializedValue : this.defaultValue;
                }

                this.initialized = true;
            }
        }

        protected virtual bool Equals(T a, T b)
        {
            if (comparer == null)
            {
                comparer = TypeExtensions.GetEqualityComparerDelegate<T>();
            }

            return comparer(a, b);
        }

        protected abstract T GetLocalValue(string key, T defaultValue);

        protected abstract void SetLocalValue(string key, T value);

        public static implicit operator T(ProjectSetting<T> projectSetting)
        {
            return projectSetting.Value;
        }

        public void Draw(Rect rect, string label, string tooltip, bool drawLocalOverride)
        {
            if (Event.current.OnContextClick(rect))
            {
                var menu = new GenericMenu();

                if (this.LocalOverride)
                    menu.AddItem(new GUIContent("Remove local override"), false, () => { this.LocalOverride = false; });
                else
                    menu.AddDisabledItem(new GUIContent("Remove local override"));

                if (this.changedFromDefault)
                    menu.AddItem(new GUIContent("Reset to default"), false, () => { this.Reset(); });
                else
                    menu.AddDisabledItem(new GUIContent("Reset to default"));

                menu.ShowAsContext();
            }

            var disableGUI = this.LocalOverride && !drawLocalOverride;
            var boldLabel = !disableGUI && ((this.LocalOverride == drawLocalOverride) && this.changedFromDefault || this.LocalOverride);
            var lbl = GUIHelper.TempContent(label, tooltip);

            GUIHelper.PushGUIEnabled(!disableGUI);
            GUIHelper.PushIsBoldLabel(boldLabel);
            {
                EditorGUI.BeginChangeCheck();

                var newValue = this.Draw(rect, this.Value, lbl);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(this.serializedContainer);

                    if (drawLocalOverride)
                    {
                        this.LocalOverride = true;
                        this.Value = newValue;
                    }
                    else
                    {
                        this.LocalOverride = false;
                        this.Value = newValue;
                    }
                }
            }
            GUIHelper.PopIsBoldLabel();
            GUIHelper.PopGUIEnabled();
        }

        protected abstract T Draw(Rect rect, T value, GUIContent label);
    }

    [Serializable]
    public sealed class ProjectSettingString : ProjectSetting<string>
    {
        protected override string GetLocalValue(string key, string defaultValue)
        {
            return EditorPrefs.GetString(key, defaultValue);
        }

        protected override void SetLocalValue(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        protected override string Draw(Rect rect, string value, GUIContent label)
        {
            return SirenixEditorFields.TextField(rect, label, value);
        }
    }

    [Serializable]
    public class ProjectSettingInt : ProjectSetting<int>
    {
        protected override int GetLocalValue(string key, int defaultValue)
        {
            return EditorPrefs.GetInt(key, defaultValue);
        }

        protected override void SetLocalValue(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        protected override int Draw(Rect rect, int value, GUIContent label)
        {
            return SirenixEditorFields.IntField(rect, label, value);
        }
    }

    [Serializable]
    public sealed class ProjectSettingBool : ProjectSetting<bool>
    {
        protected override bool GetLocalValue(string key, bool defaultValue)
        {
            return EditorPrefs.GetBool(key, defaultValue);
        }

        protected override void SetLocalValue(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        protected override bool Draw(Rect rect, bool value, GUIContent label)
        {
            return EditorGUI.ToggleLeft(rect, label, value);
        }
    }

    [Serializable]
    public sealed class ProjectSettingFloat : ProjectSetting<float>
    {
        protected override float GetLocalValue(string key, float defaultValue)
        {
            return EditorPrefs.GetFloat(key, defaultValue);
        }

        protected override void SetLocalValue(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        protected override float Draw(Rect rect, float value, GUIContent label)
        {
            return SirenixEditorFields.FloatField(rect, label, value);
        }
    }

    [Serializable]
    public class ProjectSettingEnum<T> : ProjectSetting<T> where T : struct
    {
        static ProjectSettingEnum()
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException(typeof(T).GetNiceName() + " is not an enum.");
            }
        }

        protected override T GetLocalValue(string key, T defaultValue)
        {
            var str = EditorPrefs.GetString(key, Convert.ToInt64(defaultValue).ToString("D", CultureInfo.InvariantCulture));
            long parsedValue;

            if (!long.TryParse(str, out parsedValue))
            {
                parsedValue = 0;
            }

            return (T)Enum.ToObject(typeof(T), parsedValue);
        }

        protected override void SetLocalValue(string key, T value)
        {
            EditorPrefs.SetString(key, Convert.ToInt64(value).ToString("D", CultureInfo.InvariantCulture));
        }

        protected override T Draw(Rect rect, T value, GUIContent label)
        {
            throw new NotImplementedException();
        }
    }
}
#endif