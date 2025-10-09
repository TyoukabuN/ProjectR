using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PJR.Dev.Lab.ResourceTest
{
    public class ResourceTest : MonoBehaviour
    {
        public ConditionalWeakTable<GameObject, AnyHandle> WeakTable;
    }

    public class AnyHandle : IDisposable
    {
        public void Dispose()
        {
            Debug.Log("In AnyHandle.Dispose()");
        }
    }
}
