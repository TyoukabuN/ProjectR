using PJR;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class AnimationTest : MonoBehaviour
{
    public Transform target;

    public AnimationClip animationClip;

    public float elapsedTime = 0f;

    private void Update()
    {
        if (animationClip == null)
            return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > animationClip.length)
        {
            elapsedTime = 0f;
        }
        CurveTest(elapsedTime);
    }


    public void CurveTest(float animationTime)
    {
        Vector3 localPosition = Vector3.zero;
        Quaternion localRotation = Quaternion.identity;
        Vector3 localScale = Vector3.zero;
        if (animationClip != null)
        {
            if (AnimationUtil.TryGetTranssformCurveHandle(animationClip, "root", out var handle))
            {
                handle.Evaluate(target, animationTime);
            }
        }
    }

    [Button("测试")]
    public void Test()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
        }
    }
}