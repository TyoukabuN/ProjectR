using System;
using UnityEngine;

namespace PJR.Profile
{
    public class Cost : IDisposable
    {
        public string name = String.Empty;
        private DateTime beginStamp;

        public Cost()
        {
            beginStamp = DateTime.Now;
        }
        public Cost(string name) : this()
        {
            this.name = name;
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(name))
                Debug.Log($"[Cost]: {(DateTime.Now - beginStamp).TotalMilliseconds} ms");
            else
                Debug.Log($"[Cost][{name}]: {(DateTime.Now - beginStamp).TotalMilliseconds} ms");
        }
    }
}