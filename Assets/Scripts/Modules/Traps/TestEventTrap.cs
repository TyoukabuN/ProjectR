using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;
public class TestEventTrap : ITrapEvent
{
    public void Recive(object obj)
    {
        Debug.LogError(obj.ToString()+ "TestEventTrap反射成功了？");
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