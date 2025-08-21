using System.Collections.Generic;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// 配置/持久化用
    /// Release的实现为空
    /// </summary>
    public class PersistentDataPackage : DataPackage
    {
        public override bool IsTemp => false;
        public PersistentDataPackage()
        {
            _dataMap = new Dictionary<DataType, DataValue>();
        }
        public override TempDataPackage GetTemplate()
        {
            return TempDataPackage.Get(this);
        }
        public override void Release()
        {
        }
    }
}