using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class TrapConfigHost : EntityConfigHost
    {
        public override EntityConfigAsset EntiyConfigAsset => configAsset;

        [InlineEditor]
        public TrapConfigAsset configAsset;
    }
}
