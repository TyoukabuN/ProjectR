using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrap : MonoBehaviour,ITrap
{
    public float height;
    public float width;
    public Collider box;
    public void OnStart()
    {
        Debug.Log("wallTrap start");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
