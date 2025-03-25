using System.Collections.Generic;

namespace PJR
{
    public interface INumericalControl
    {
        public abstract Dictionary<string, ExtraValue> ExtraValueMap { get; set; }
    }
}