using System.Collections.Generic;
using UnityEngine;

namespace PJR.ClassExtension
{
    public static class TransformExtension
    {
        /// <summary>
        /// 设置Parent并根据给定Anchor位置设置偏移
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent"></param>
        /// <param name="anchor"></param>
        public static void SetParentWithAnchor(this Transform transform, Transform parent, Transform anchor)
        {
            var rotInv_handle = Quaternion.Inverse(anchor.rotation);

            transform.rotation = parent.rotation * rotInv_handle * transform.rotation;
            transform.position = parent.position + transform.position - anchor.position;
            transform.SetParent(parent, worldPositionStays: true);
        }

        /// <summary>
        /// 获取Hierarchy路径
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static string CopyHierarchyPath(this Transform transform)
        {
            var path = string.Empty;
            if (transform == null)
                return path;
            if (transform.parent == null)
                path = transform.name;
            else
            {
                path = CopyHierarchyPath(transform.parent);
                if (!string.IsNullOrEmpty(path)) path = $"{path}/{transform.name}";
            }
            return path;
        }
        /// <summary>
        /// 获取Transform下所有Child
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static List<Transform> GetChilds(this Transform transform, int depth = 1)
        {
            List<Transform> childList = new List<Transform>();
            GetChilds_Recursion(transform, depth, childList);
            return childList;
        }

        private static void GetChilds_Recursion(Transform transform, int depth, List<Transform> list)
        {
            if (depth <= 0)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                list.Add(transform.GetChild(i));
                GetChilds_Recursion(transform.GetChild(i), depth - 1, list);
            }
        }
        /// <summary>
        /// 重置Transform参数
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetValue(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
    }
}