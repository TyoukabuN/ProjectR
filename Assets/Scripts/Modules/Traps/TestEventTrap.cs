using System.Collections.Generic;
using PJR;
using UnityEngine;

public class TestEventTrap : ITrapEvent
{
    public void Recive(object obj)
    {
        Debug.LogError(obj.ToString()+ "TestEventTrap����ɹ��ˣ�");
    }

}
public class TrapEventM: MonoSingleton<TrapEventM>
{
    public List<ITrapEvent> TrapEvents = new List<ITrapEvent>();
    public void Excute(object obj)
    {
        foreach (var trap in TrapEvents)
        {
            trap.Recive(obj);
        }
    }
}