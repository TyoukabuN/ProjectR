using System.Collections.Generic;

namespace PJR
{
    /// <summary>
    /// 提供类似Shader.PropertyToID的服务
    /// </summary>
    public class StringUID
    {
        private static readonly object _mapLock = new object(); // buffer锁对象
        private static Dictionary<string, int> _propertyToIDMap = new();
        private static int _uid = 0;

        public static int PropertyToID(string propertyName)
        {
            int id = 0;
            if (string.IsNullOrEmpty(propertyName)) 
                return id;

            lock (_mapLock) {
                if (!_propertyToIDMap.TryGetValue(propertyName, out id)) 
                {
                    id = ++_uid;
                    _propertyToIDMap[propertyName] = id;
                }
            }
            return id;
        }
    }
}
