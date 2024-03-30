using System;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor
{
    public class ContextedDialogWindow :OdinEditorWindow
    {

        private static GUIStyle BigFontMessageBox
        {
            get
            {
                if (bigFontMessageBox == null)
                {
                    bigFontMessageBox = new GUIStyle(SirenixGUIStyles.MessageBox);
                    bigFontMessageBox.fontSize = 14;
                }

                return bigFontMessageBox;
            }
        }
        private static GUIStyle bigFontMessageBox;
        
        #region Wrappers

        [Serializable]
        private class EmptyContextWrapper
        {
            [HideInInspector]
            public string title;
            [HideInInspector]
            public string info;

            [OnInspectorGUI][PropertyOrder(-1000)]
            private void Draw()
            {
                var t = string.IsNullOrWhiteSpace(title) ? "你好哇" : title;
                SirenixEditorGUI.Title(t, null, TextAlignment.Left, horizontalLine:true, true);
                if (string.IsNullOrWhiteSpace(info)) return;
                var rect = GUILayoutUtility.GetRect(GUIHelper.TempContent(info), BigFontMessageBox);
                rect = EditorGUI.IndentedRect(rect);
                GUI.Label(rect, GUIHelper.TempContent(info), BigFontMessageBox);
            }
            
            [HideInInspector]
            public Action onConfirm;
            [HideInInspector]
            public string confirmStr;

            public string ConfirmStr => string.IsNullOrWhiteSpace(confirmStr) ? "确认" : confirmStr;
            [HideInInspector]
            public OdinEditorWindow window;
            [Button("$ConfirmStr")][HorizontalGroup(0.5f)]
            public void Confirm()
            {
                onConfirm?.Invoke();
                window?.Close();
            }
            [Button("取消")][HorizontalGroup]
            public void Cancel()
            {
                window?.Close();
            }
        }
        [Serializable]
        public class ContextWrapper<T>
        {
            [DisplayAsString][HideLabel][HideIf("infoIsEmpty")]
            public string info;
            private bool infoIsEmpty => string.IsNullOrWhiteSpace(info);
            [LabelText("$label")][InlineProperty][ShowIf("$hasContext")]
            public T contextObject;
            [HideInInspector]
            public string label;
            [HideInInspector]
            public Action<T> onConfirm;
            [HideInInspector]
            public string confirmStr;
            [HideInInspector]
            public bool hasContext;

            public string ConfirmStr => string.IsNullOrWhiteSpace(confirmStr) ? "确认" : confirmStr;
            [HideInInspector]
            public OdinEditorWindow window;
            [Button("ConfirmStr")][HorizontalGroup(0.5f)]
            public void Confirm()
            {
                onConfirm?.Invoke(contextObject);
                window?.Close();
            }
            [Button("取消")][HorizontalGroup]
            public void Cancel()
            {
                window?.Close();
            }
        }
        #endregion

        public static OdinEditorWindow ShowDialog(Action onConfirm, string title=null,string info = null, string confirmStr = null)
        {
            var wrapper = new EmptyContextWrapper
            {
                title = title,
                info = info,
                onConfirm = onConfirm,
                confirmStr = confirmStr,
            };
            var position = GUIHelper.CurrentWindow.position.center;
            var btnRect = new Rect(position.x, position.y, 1, 1);
            var window = OdinEditorWindow.InspectObjectInDropDown(wrapper/*, btnRect, size_*/);
            wrapper.window = window;
            return window;
        }
        
        public static OdinEditorWindow ShowContextedDialog<T>(T context, Action<T> onConfirm, string info = null, string contextLabel = null, string confirmStr = null)
        {
            var wrapper = new ContextWrapper<T>
            {
                info = info,
                contextObject = context,
                label = contextLabel,
                onConfirm = onConfirm,
                confirmStr = confirmStr,
                hasContext = true,
            };
            var window = OdinEditorWindow.InspectObjectInDropDown(wrapper);
            wrapper.window = window;
            return window;
        }
    }
}
