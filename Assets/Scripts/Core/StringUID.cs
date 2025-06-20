using System.Collections.Generic;

namespace PJR
{
    /// <summary>
    /// 提供类似Shader.PropertyToID的服务
    /// </summary>
    public class StringUid
    {
        private Dictionary<string, int> _propertyToIDMap = new();
        private int _uid = 0;

        public int PropertyToID(string propertyName) {
            if (string.IsNullOrEmpty(propertyName)) 
                return 0;

            if (_propertyToIDMap.TryGetValue(propertyName, out int id))
                return id;
    
            lock (_propertyToIDMap) {
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
