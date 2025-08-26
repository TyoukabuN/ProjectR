using PJR.Core.TypeExtension;
using UnityEngine.Pool;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// for runtime get
    /// </summary>
    public class TempDataPackage : DataPackage
    {
        public override bool IsTemp => true;
        public TempDataPackage()
        {
        }
        public static TempDataPackage Get()
        {
            var temp = GenericPool<TempDataPackage>.Get();
            temp._dataMap = DictionaryPool<DataType, DataValue>.Get();
            return temp;
        }
        public static TempDataPackage Get(IDataPackage rhs)
        {
            var temp = GenericPool<TempDataPackage>.Get();
            temp.Add(rhs);
            return temp;
        }
        public override TempDataPackage GetTemplate() => this;
        public override void Release()
        {
            _dataMap?.Release();
            GenericPool<TempDataPackage>.Release(this);
        }
    }
}