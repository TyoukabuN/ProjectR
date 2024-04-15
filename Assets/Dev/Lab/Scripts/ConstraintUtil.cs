using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public static class ConstraintUtil
{
    public static bool TryAddParentConstraint(Transform attached, Transform parent, Transform target,Vector3 posOffset,Vector3 rotOffset)
    {
        if(attached == null)
            return false;

        var gameObject = attached.gameObject;

        if (!gameObject.TryGetComponent<ParentConstraint>(out var parentConstraint))
        {
            parentConstraint = gameObject.AddComponent<ParentConstraint>();
        }
        var source = new ConstraintSource()
        {
            sourceTransform = parent,
            weight = 1f,
        };
        if (parentConstraint.sourceCount <= 0)
            parentConstraint.AddSource(source);
        else
            parentConstraint.SetSource(0, source);
        parentConstraint.SetTranslationOffset(0, posOffset);
        parentConstraint.SetRotationOffset(0, rotOffset);

        parentConstraint.constraintActive = true;
        parentConstraint.weight = 1f;

        return parentConstraint;
    }
}
