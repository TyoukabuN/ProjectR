using PJR;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class HitBox : MonoBehaviour
{
    public bool InCollision = false;
    public Transform anchor = null;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        if (collision.contacts.Count() > 0)
        {
            InCollision = true;
            AddContraint(collision);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Count() > 0)
        {
            InCollision = true; 
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        InCollision = false;
        CloseContraint();
    }
    private ParentConstraint parentConstraint = null;
    public void AddContraint(Collision collision)
    {
        var parent = collision.collider.transform;
        if (parent.tag != "Player")
            return;
        Debug.Log(parent);
        var collisionPoint = collision.contacts[0].point;
        Vector3 positionOffset = collision.collider.transform.worldToLocalMatrix * collisionPoint;

        var anchorOffset = transform.position - anchor.transform.position;

        parentConstraint = gameObject.TryGetComponent<ParentConstraint>();

        var source = new ConstraintSource()
        {
            sourceTransform = parent,
            weight = 1f,
        };
        if (parentConstraint.sourceCount <= 0)
            parentConstraint.AddSource(source);
        else
            parentConstraint.SetSource(0, source);
        parentConstraint.SetTranslationOffset(0, positionOffset + anchorOffset);
        parentConstraint.SetRotationOffset(0, transform.rotation.eulerAngles);

        parentConstraint.constraintActive = true;
        parentConstraint.weight = 1f;
    }
    public void CloseContraint()
    {
        if (parentConstraint != null)
        { 
            parentConstraint.constraintActive = false;
        }
    }
}
