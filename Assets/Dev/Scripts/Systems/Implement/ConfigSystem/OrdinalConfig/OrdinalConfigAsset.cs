using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LS.Game
{
    public abstract class OrdinalConfigAsset<TItemAsset, TItem> : OrdinalConfigAsset<TItemAsset>
        where TItemAsset : OrdinalConfigItemAsset<TItem> 
        where TItem : OrdinalConfigItem
    {
    }

    public abstract class OrdinalConfigAsset<TItemAsset> : SerializedScriptableObject
        where TItemAsset : IOrdinalConfigItem
    {
        [ListDrawerSettings(CustomAddFunction = "Editor_CustomAddFunction")]
        public List<TItemAsset> itemAssets;

        public virtual void Editor_CustomAddFunction()
        {
            Debug.Log($"需要重写{nameof(OrdinalConfigAsset<TItemAsset>)}.Editor_CustomAddFunction");
        }
    }
}
