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

    public Transform attached = null;
    public Transform attached_offset = null;

    public bool usingAnchor = false;

    private GameObject _own = null;

    [Button("AttachedTest")]
    public void Attached()
    {
        Clear();

        if (!EditorApplication.isPlaying)
            return;

        _own = GameObject.Instantiate(own.gameObject);

        Vector3 point = attached_offset.position;

        if (attached != null)
        {
            Vector3 posOffset = Vector3.zero;
            Vector3 rotOffset = _own.transform.eulerAngles;
            var anchor = _own.GetComponentInChildren<Anchor>()?.transform;


            //Vector3 vec3 =  attached.worldToLocalMatrix * new Vector4(point.x, point.y, point.z,1);
            Vector3 vec3 =  point - attached.position;
            vec3 = attached.rotation * vec3;


            posOffset += vec3;
            //posOffset += attached.worldToLocalMatrix.MultiplyPoint(point);

            if (usingAnchor && anchor != null)
            {
                //posOffset += attached.worldToLocalMatrix.MultiplyVector(_own.transform.position - anchor.position);
                vec3 = _own.transform.position - anchor.position;
                vec3 = attached.worldToLocalMatrix * vec3;
                //vec3 = attached.rotation * vec3;
                posOffset += vec3;
                //rotOffset = (anchor.rotation * Quaternion.Euler(rotOffset.x, rotOffset.y, rotOffset.z)).eulerAngles;
            }


            if (TryAddParentConstraint(_own.transform, attached, posOffset, rotOffset, out var inst))
            {
                parentConstraint = inst;
            }
        }
    }

    private void Update()
    {
        if (attached_offset != null)
        {
            Debug.Log(attached_offset.position);
            Debug.Log(attached.worldToLocalMatrix);
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
