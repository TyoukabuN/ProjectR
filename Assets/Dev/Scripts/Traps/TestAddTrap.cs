using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TestAddTrap : MonoBehaviour
{
    [Button("添加一个陷阱")]
    public void Add()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = Vector3.zero;
        WallTrap trap = obj.AddComponent<WallTrap>();
        TrapManager.instance.AddTrap(trap);
    }
    [Button("机关开始")]
    public void Func()
    {
        ITrap t = TrapManager.instance.NextTrap();
        t.OnStart();
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
