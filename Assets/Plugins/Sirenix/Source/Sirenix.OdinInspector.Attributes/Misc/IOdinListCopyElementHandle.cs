using Sirenix.OdinInspector;
using System;

namespace Sirenix.OdinInspector
{
    /// <summary>
    /// 复制条目时，有没有需要特殊处理的属性？覆写此接口
    /// </summary>
    public interface IOdinListCopyElementHandle
    {
        public void CopyCallBack(ref object copiedObj);
    }
}