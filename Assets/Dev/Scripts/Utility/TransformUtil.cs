using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class TransformUtil
    {
        public static TransformValueRecord RecordTransformValue(Transform transform)
        {
            return new TransformValueRecord(transform);
        }
    }
    public class TransformValueRecord
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public Transform from;

        public bool Valid => from != null;
        public TransformValueRecord(Transform transform)
        {
            from = transform;
            if (!Valid)
                return;
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;
        }
        public void Revert(Transform transform)
        {
            if (!Valid)
                return;
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
        }
    }
}