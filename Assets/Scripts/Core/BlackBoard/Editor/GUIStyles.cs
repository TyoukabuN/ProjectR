using UnityEngine;
using UnityEditor;

namespace PJR.BlackBoard.Editor
{
    public static class GUIStyles
    {
        private static GUIStyle _minirichTextButton;

        public static GUIStyle MiniRichTextButton
        {
            get
            {
                if (_minirichTextButton == null)
                {
                    _minirichTextButton = new GUIStyle(EditorStyles.miniButton);
                    _minirichTextButton.richText = true;
                    _minirichTextButton.padding = new RectOffset(0, 0, 0, 0);
                    _minirichTextButton.alignment = TextAnchor.MiddleCenter;
                }

                return _minirichTextButton;
            }
        }
    }
}
