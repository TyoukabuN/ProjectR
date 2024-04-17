using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;

public class ConstraintTest : MonoBehaviour
{
    public Transform own = null;
    public Transform own_anchor = null;

    public Transform target = null;
    public Transform target_offset = null;

    public bool usingAnchor = false;

    private GameObject _own = null;

    [Button("AttachedTest")]
    public void Attached()
    {
        Clear();

        if (!EditorApplication.isPlaying)
            return;

        _own = GameObject.Instantiate(own.gameObject);

        Vector3 offset = target_offset.position;

        if (target != null)
        {
            Vector3 posOffset = Vector3.zero;
            //Vector3 rotOffset = _own.transform.eulerAngles;
            var targetRot = target.transform.rotation;
            var targetRotInverse = Quaternion.Inverse(targetRot);

            Vector3 rotOffset = (targetRotInverse * _own.transform.rotation).eulerAngles;

            var anchor = _own.GetComponentInChildren<Anchor>()?.transform;
            if (usingAnchor && anchor != null)
            {
                //rotOffset = (targetRotInverse * anchor.rotation * _own.transform.rotation).eulerAngles;
            }

            posOffset += targetRotInverse * (offset - target.position);

            if (usingAnchor && anchor != null)
            {
                posOffset += targetRotInverse * (_own.transform.position - anchor.position);
            }


            if (TryAddParentConstraint(_own.transform, target, posOffset, rotOffset, out var inst))
            {
                parentConstraint = inst;
            }
        }
    }

    private void Update()
    {
        if (target_offset != null)
        {
            Debug.Log(target_offset.position);
            Debug.Log(target.worldToLocalMatrix);
        }
    }

    private ParentConstraint parentConstraint;
    public bool TryAddParentConstraint(Transform attached, Transform parent, Vector3 posOffset, Vector3 rotOffset, out ParentConstraint parentConstraint)
    {
        parentConstraint = null;
        if (attached == null)
            return false;

        var gameObject = attached.gameObject;

        if (!gameObject.TryGetComponent<ParentConstraint>(out parentConstraint))
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

        return true;
    }
    public void Clear()
    {
        if (_own != null)
        {
            GameObject.DestroyImmediate(_own);
            _own = null;
        }
    }
}
