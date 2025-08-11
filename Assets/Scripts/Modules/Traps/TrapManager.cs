using System.Collections.Generic;
using PJR;

public class TrapManager : MonoSingleton<TrapManager>
{
    public List<ITrap> trapList = new List<ITrap>();
    public Queue<ITrap> trapQueue = new Queue<ITrap>();
    // Start is called before the first frame update
    void Start()
    {
        trapList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTrap(ITrap trap)
    {
        trapQueue.Enqueue(trap);
    }
    public ITrap NextTrap()
    {
        if (trapQueue.Count >0)
        {
            ITrap trap = trapQueue.Dequeue();
            trapList.Add(trap);
            return trap;
        }
        return null;
    }
    public Queue<ITrap> GetTrapQueue()
    {
        return trapQueue;
    }
}
public enum TrapType
{
    Wall,
}
