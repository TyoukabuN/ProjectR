using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoInst : MonoBehaviour
{
    [ShowInInspector]
    public CSInst CSInst = null;
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Handle(GameObject gobj)
    {
        CSInst = CSInst ?? new CSInst(gobj);
    }
}

public class CSInst
{
    public int value = 1;
    public CSInst(GameObject gobj)
    { 
    }
    ~CSInst()
    {
        Debug.Log($"[CSInst.Destructor]");
        //if (gobj != null)
        //    gobj = null;
    }
}
