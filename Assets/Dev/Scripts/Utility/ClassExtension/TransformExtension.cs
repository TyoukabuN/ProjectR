using Animancer.Examples.StateMachines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public static class TransformExtension
{
    public static void SetParentWithAnchor(this Transform transform,Transform parent,Transform anchor)
    {
        var rotInv_handle = Quaternion.Inverse(anchor.rotation);

        transform.rotation = parent.rotation * rotInv_handle * transform.rotation;
        transform.position = parent.position + transform.position - anchor.position;
        transform.SetParent(parent, worldPositionStays: true);
    }
}
