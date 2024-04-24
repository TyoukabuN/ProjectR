using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationUtil
{
    private static Dictionary<AnimationClip, TransformCurveHandle> clip2transCurveHandle = new Dictionary<AnimationClip, TransformCurveHandle>();

    public static bool TryGetTranssformCurveHandle(AnimationClip animationClip, string path, out TransformCurveHandle handle)
    {
        handle = null;
        path = string.IsNullOrEmpty(path) ? "root" : path;
        if (animationClip == null)
            return false; ;
        clip2transCurveHandle ??= new Dictionary<AnimationClip, TransformCurveHandle>();
        if (!clip2transCurveHandle.TryGetValue(animationClip, out handle))
        {
            handle = new TransformCurveHandle(animationClip, path);
            clip2transCurveHandle[animationClip] = handle;
        }

        return true;
    }
    /// <summary>
    /// 直接获取AnimationClip中root节点Transform相关的curve来实现rootMotion
    /// 其他地方应该可以用到，但是先放在这里
    /// </summary>
    public class TransformCurveHandle
    {
        public AnimationCurve localPositionX;
        public AnimationCurve localPositionY;
        public AnimationCurve localPositionZ;
        public AnimationCurve localRotationX;
        public AnimationCurve localRotationY;
        public AnimationCurve localRotationZ;
        public AnimationCurve localRotationW;
        public AnimationCurve localScaaleX;
        public AnimationCurve localScaaleY;
        public AnimationCurve localScaaleZ;

        public TransformCurveHandle(AnimationClip animationClip, string path)
        {
            if (animationClip == null || string.IsNullOrEmpty(path))
                return;
            var editorCurveBindings = AnimationUtility.GetCurveBindings(animationClip);
            foreach (var curveBinding in editorCurveBindings)
            {
                if (curveBinding.type != typeof(Transform))
                    continue;
                if (curveBinding.path != path)
                    continue;

                var curve = AnimationUtility.GetEditorCurve(animationClip, curveBinding);

                if (curveBinding.propertyName == "m_LocalPosition.x") localPositionX = curve;
                if (curveBinding.propertyName == "m_LocalPosition.y") localPositionY = curve;
                if (curveBinding.propertyName == "m_LocalPosition.z") localPositionZ = curve;

                if (curveBinding.propertyName == "m_LocalRotation.x") localRotationX = curve;
                if (curveBinding.propertyName == "m_LocalRotation.y") localRotationY = curve;
                if (curveBinding.propertyName == "m_LocalRotation.z") localRotationZ = curve;
                if (curveBinding.propertyName == "m_LocalRotation.w") localRotationW = curve;

                if (curveBinding.propertyName == "m_LocalScale.x") localScaaleX = curve;
                if (curveBinding.propertyName == "m_LocalScale.y") localScaaleY = curve;
                if (curveBinding.propertyName == "m_LocalScale.z") localScaaleZ = curve;
            }
        }

        public void Evaluate(Transform target, float time)
        {
            if (target == null) return;

            Vector3 localPosition = Vector3.zero;
            Quaternion localRotation = Quaternion.identity;
            Vector3 localScale = Vector3.zero;

            if (localPositionX != null) localPosition.x = localPositionX.Evaluate(time);
            if (localPositionY != null) localPosition.y = localPositionY.Evaluate(time);
            if (localPositionZ != null) localPosition.z = localPositionZ.Evaluate(time);

            if (localRotationX != null) localRotation.x = localRotationX.Evaluate(time);
            if (localRotationY != null) localRotation.y = localRotationY.Evaluate(time);
            if (localRotationZ != null) localRotation.z = localRotationZ.Evaluate(time);
            if (localRotationW != null) localRotation.w = localRotationW.Evaluate(time);

            if (localScaaleX != null) localScale.x = localScaaleX.Evaluate(time);
            if (localScaaleY != null) localScale.y = localScaaleY.Evaluate(time);
            if (localScaaleZ != null) localScale.z = localScaaleZ.Evaluate(time);

            target.localPosition = localPosition;
            target.localRotation = localRotation;
            target.localScale = localScale;
        }
    }
}
