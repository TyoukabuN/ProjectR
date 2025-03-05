using System.Collections.Generic;
using Sirenix.OdinInspector;

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
        public List<TItemAsset> itemAssets;
    }
}
