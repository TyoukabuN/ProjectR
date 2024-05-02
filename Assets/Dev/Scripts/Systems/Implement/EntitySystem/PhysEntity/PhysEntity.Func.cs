using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace PJR
{
    public partial class PhysEntity : MonoBehaviour
    {
        public void SetPosition(Vector3 pos)
        {
            motor?.SetPosition(pos);
        }
    }
}
