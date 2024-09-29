using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class ResourceReference
    {
        public string info = string.Empty;
        public string timeStamp = string.Empty;
        ~ResourceReference() {
            Debug.Log("[ResourceReference.Destructor] Destructor");
        }
    }
}