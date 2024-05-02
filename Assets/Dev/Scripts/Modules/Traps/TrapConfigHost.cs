using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class TrapConfigHost : EntityConfigHost
    {
        [InlineEditor]
        public TrapConfigAsset configAsset;
        //手动控制生命周期
        public bool isManual = false; 
    }
}
