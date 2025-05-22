#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace PJR.ClassExtension
{
    public static class GenericMenuExtension
    {
        public static void AddItem(this GenericMenu menu, string contentStr, bool on, GenericMenu.MenuFunction func)
        {
            menu.AddItem(new GUIContent(contentStr), on, func);
        }
    }
}

#endif
