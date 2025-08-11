using Sirenix.OdinInspector;
using UnityEngine;

public class TestAddTrap : MonoBehaviour
{
    [Button("���һ������")]
    public void Add()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = Vector3.zero;
        WallTrap trap = obj.AddComponent<WallTrap>();
        TrapManager.instance.AddTrap(trap);
    }
    [Button("���ؿ�ʼ")]
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
