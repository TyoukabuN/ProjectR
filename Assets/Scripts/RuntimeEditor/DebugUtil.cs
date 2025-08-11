using System;
using UnityEngine;
using UnityEngine.Profiling;

public struct ProfileScope : IDisposable
{
    public string name;
    public ProfileScope(string name) 
    {
        this.name = name;
        Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        Profiler.EndSample();
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