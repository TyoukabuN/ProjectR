using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
    protected virtual void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay");
    }
    protected virtual void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit");
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
    }
}
