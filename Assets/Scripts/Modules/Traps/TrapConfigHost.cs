using Sirenix.OdinInspector;

namespace PJR
{
    public class TrapConfigHost : EntityConfigHost
    {
        public override EntityConfigAsset EntiyConfigAsset => configAsset;

        [InlineEditor]
        public TrapConfigAsset configAsset;
    }
}
