using System;
using UnityEngine;

public class ProfileScope : System.IDisposable
{
    public string name = String.Empty;
    private DateTime beginStamp;

    private ProfileScope()
    {
        beginStamp = System.DateTime.Now;
    }
    public ProfileScope(string name) : this()
    {
        this.name = name;
        UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}

public class MemoryCostScope : IDisposable
{
    public string name = String.Empty;
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