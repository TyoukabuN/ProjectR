using Sirenix.OdinInspector;

namespace PJR
{
    public class ItemConfigHost : EntityConfigHost
    {
        public override EntityConfigAsset EntiyConfigAsset => configAsset;

        [InlineEditor]
        public ItemConfigAsset configAsset;
    }
}