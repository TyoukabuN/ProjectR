using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Shooter : MonoBehaviour
{
    public GameObject bullet;
    public GameObject target;
    public float radius = 3f;
    public float forceMag = 3f;

    public Avatar avatar;

    [Button("Shoot")]
    public void Shoot()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            return;
#endif
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
