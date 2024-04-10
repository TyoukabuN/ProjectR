using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Profile
{
    public class Cost : System.IDisposable
    {
        public string name = String.Empty;
        private DateTime beginStamp;

        public Cost()
        {
            beginStamp = System.DateTime.Now;
        }
        public Cost(string name) : this()
        {
            this.name = name;
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(name))
                Debug.Log($"[Cost]: {(System.DateTime.Now - beginStamp).TotalMilliseconds} ms");
            else
                Debug.Log($"[Cost][{name}]: {(System.DateTime.Now - beginStamp).TotalMilliseconds} ms");
        }
    }
}