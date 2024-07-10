using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TransformExtension
{
    public static void SetParentWithAnchor(this Transform transform,Transform parent,Transform anchor)
    {
        var rotInv_handle = Quaternion.Inverse(anchor.rotation);

        transform.rotation = parent.rotation * rotInv_handle * transform.rotation;
        transform.position = parent.position + transform.position - anchor.position;
        transform.SetParent(parent, worldPositionStays: true);
    }

    public static string CopyPath(this Transform transform)
    {
        var path = string.Empty;
        if (transform == null)
            return path;
        if (transform.parent == null)
            path = transform.name;
        else
        {
            path = CopyPath(transform.parent);
            if (!string.IsNullOrEmpty(path)) path = $"{path}/{transform.name}";
        }
        return path;
    }
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
}
