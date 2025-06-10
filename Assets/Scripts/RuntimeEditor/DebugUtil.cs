using System;
using UnityEngine;

public struct ProfileScope : System.IDisposable
{
    public string name;
    public ProfileScope(string name) 
    {
        this.name = name;
        UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}

public struct MemoryCostScope : IDisposable
{
    public string name;
    public long beginStamp;
    public MemoryCostScope(string name)
    {
        this.name = name;
        beginStamp = GC.GetTotalMemory(true);
    }
    public void Dispose()
    {
        long res = (GC.GetTotalMemory(false) - beginStamp) / 1000;
        Debug.Log($"[Cost][{name}] : [{res}] kb");
    }
}