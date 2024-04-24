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
        CurveTest();
    }


    [Button(nameof(CurveTest))]
    public void CurveTest()
    {


        Vector3 localPosition = Vector3.zero;
        Quaternion localRotation = Quaternion.identity;
        Vector3 localScale = Vector3.zero;
        if (animationClip != null)
        {
            var editorCurveBindings = AnimationUtility.GetCurveBindings(animationClip);
            foreach (var curveBinding in editorCurveBindings)
            {
                if (curveBinding.type != typeof(Transform))
                    continue;
                if (curveBinding.path == "root")
                { 
                    Debug.Log($"{curveBinding.path}/{curveBinding.propertyName}  Type: {curveBinding.type}");
                    var curve = AnimationUtility.GetEditorCurve(animationClip, curveBinding);

                    if (curveBinding.propertyName == "m_LocalPosition.x")
                        localPosition.x = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalPosition.y")
                        localPosition.y = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalPosition.z")
                        localPosition.z = curve.Evaluate(elapsedTime);

                    if (curveBinding.propertyName == "m_LocalRotation.x")
                        localRotation.x = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalRotation.y")
                        localRotation.y = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalRotation.z")
                        localRotation.z = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalRotation.w")
                        localRotation.w = curve.Evaluate(elapsedTime);

                    if (curveBinding.propertyName == "m_LocalScale.x")
                        localScale.x = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalScale.y")
                        localScale.y = curve.Evaluate(elapsedTime);
                    if (curveBinding.propertyName == "m_LocalScale.z")
                        localScale.z = curve.Evaluate(elapsedTime);
                }
            }
            if (target != null)
            {
                target.localPosition = localPosition;
                target.localRotation = localRotation;
            }
        }
    }
}
