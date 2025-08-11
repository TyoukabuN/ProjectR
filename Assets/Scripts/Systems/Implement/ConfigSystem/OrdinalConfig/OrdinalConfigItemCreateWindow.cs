#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJR.Config
{
    public abstract class OrdinalConfigItemCreateWindow<TConfig, TAsset, TItemAsset, TItem> : OrdinalConfigItemCreateWindow<TConfig, TAsset, TItemAsset>
        where TConfig : OrdinalConfig<TAsset, TItemAsset, TItem>
        where TAsset : OrdinalConfigAsset<TItemAsset, TItem>
        where TItemAsset : OrdinalConfigItemAsset<TItem>
        where TItem : OrdinalConfigItem, new()
    {
        public TItem item = null;

        protected override void OnGUI()
        {
            if (!Valid)
            {
                DrawInvalidMsg();
                return;
            }

            SetupItemEditor();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawItemEditor();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("确定"))
            {
                string error = string.Empty;
                if(string.IsNullOrEmpty(directory))
                    error = config.Editor_NewConfig(item);
                else
                    error = config.Editor_NewConfig(directory, item);

                if (!string.IsNullOrEmpty(error))
                {
                    EditorUtility.DisplayDialog("Tips", error, "OK");
                }
                else
                {
                    onFinish?.Invoke(itemAsset);
                    Close();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        protected override void SetupItemEditor()
        {
            if (itemAsset == null)
            {
                ClearUpEditor();

                itemAsset = CreateInstance<TItemAsset>();
                item = new TItem();
                item.ID = config.Editor_GetNewID();
                item.Name = "起码填个名字吧？";
                itemAsset.item = item;

                if (InheritsFrom(itemAsset.GetType(), typeof(Object)))
                {
                    var unityObject = itemAsset as Object;
                    if (unityObject)
                        editor = CreateObjectEditor(unityObject);
                }
                else
                {
                    if (itemAsset is IList)
                        propertyTree = PropertyTree.Create(itemAsset as IList);
                    else
                        propertyTree = PropertyTree.Create(itemAsset);
                }
            }
        }
       
    }

    public abstract class OrdinalConfigItemCreateWindow<TConfig, TAsset, TItemAsset> : EditorWindow
        where TConfig : OrdinalConfig<TAsset, TItemAsset>
        where TAsset : OrdinalConfigAsset<TItemAsset>
        where TItemAsset : OrdinalConfigItemAsset
    {
        public TConfig config = null;
        public TItemAsset itemAsset = null;
        protected PropertyTree propertyTree = null;
        protected UnityEditor.Editor editor = null;
        protected Action<TItemAsset> onFinish = null;
        protected string directory = null;
        protected bool inited = false;
        public virtual void Init(TConfig configInstance, Action<TItemAsset> onFinish = null, string directory = null)
        {
            this.config = configInstance;
            this.onFinish = onFinish;
            this.directory = directory;
            SetupItemEditor();
            inited = true;
        }

        protected Vector2 _scrollPosition = Vector2.zero;

        protected virtual bool Valid
        {
            get
            {
                if (!inited)
                    return false;
                if (config == null)
                    return false;
                return true;
            }
        }
        protected const string default_error_msg = "非法参数,GetWindow后需要Init";
        protected virtual void DrawInvalidMsg()
        {
            EditorGUILayout.HelpBox(default_error_msg, MessageType.Error);
        }
        protected virtual void OnGUI()
        {
            if (!Valid)
            {
                DrawInvalidMsg();
                return;
            }

            SetupItemEditor();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawItemEditor();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("确定"))
            {
                string error = string.Empty;
                if (string.IsNullOrEmpty(directory))
                    error = config.Editor_NewConfig(itemAsset);
                else
                    error = config.Editor_NewConfig(directory, itemAsset);

                if (!string.IsNullOrEmpty(error))
                {
                    EditorUtility.DisplayDialog("Tips", error, "OK");
                }
                else
                {
                    onFinish?.Invoke(itemAsset);
                    Close();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        protected virtual void SetupItemEditor()
        {
            if (itemAsset == null)
            {
                ClearUpEditor();

                itemAsset = CreateInstance<TItemAsset>();
                itemAsset.ID = config.Editor_GetNewID();
                itemAsset.Name = "起码填个名字吧？";

                if (InheritsFrom(itemAsset.GetType(), typeof(Object)))
                {
                    var unityObject = itemAsset as Object;
                    if (unityObject)
                        editor = CreateObjectEditor(unityObject);
                }
                else
                {
                    if (itemAsset is IList)
                        propertyTree = PropertyTree.Create(itemAsset as IList);
                    else
                        propertyTree = PropertyTree.Create(itemAsset);
                }
            }
        }
        protected virtual void ClearUpEditor()
        {
            itemAsset = null;
            if (propertyTree != null) propertyTree.Dispose();
            propertyTree = null;
            if (editor) DestroyImmediate(editor);
            editor = null;
        }
        protected virtual void DrawItemEditor()
        {
            if (propertyTree != null)
                propertyTree.Draw(false);
            else if (editor != null)
                editor.OnInspectorGUI();
        }
        protected virtual bool InheritsFrom(Type type, Type baseType)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.IsInterface && baseType.IsInterface == false)
            {
                return false;
            }

            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            var t = type;
            while (t != null)
            {
                if (t == baseType)
                {
                    return true;
                }

                if (baseType.IsGenericTypeDefinition && t.IsGenericType && t.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }

                t = t.BaseType;
            }

            return false;
        }

        protected virtual void OnDisable()
        {
            ClearUpEditor();
        }

        protected virtual UnityEditor.Editor CreateObjectEditor(Object target)
        {
            return UnityEditor.Editor.CreateEditor(target);
        }
    }
}
#endif
