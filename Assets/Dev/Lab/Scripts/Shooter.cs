using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bullet;
    public GameObject target;
    public float radius = 3f;
    public float forceMag = 3f;

    [Button("Shoot")]
    public void Shoot()
    {
        if (!EditorApplication.isPlaying)
            return;
        Vector3 random = Random.onUnitSphere;
        var genSpot = random * radius + target.transform.position;
        var dir = -random;

        var inst = GameObject.Instantiate(bullet);
        inst.transform.position = genSpot;
        inst.transform.up = random;

        Rigidbody rig = inst.GetComponent<Rigidbody>();
        if (rig)
        {
            rig.AddForce(dir * forceMag);
        }
    }
}
