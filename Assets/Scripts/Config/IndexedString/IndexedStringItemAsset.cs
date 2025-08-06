using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Config
{
    public class IndexedStringItemAsset : OrdinalConfigItemAssetTemplate
    {
        [SerializeField]
        [LabelText("字符串值")]
        private string _stringValue;
        [SerializeField]
        [LabelText("哈希值")]
        [InlineButton("Editor_RecalculateHash_Btn","重新计算")]
        private int _hashCode;

        public string StringValue => _stringValue;
        public int hashCode => _hashCode;

        public override bool Valid
        {
            get
            {
                if (string.IsNullOrEmpty(_stringValue))
                    return false;
                if (_hashCode <= 0)
                    return false;
                return base.Valid;
            }
        }
        public override string Name => _stringValue;

#if UNITY_EDITOR
        public void Editor_RecalculateHash_Btn()
        {
            Editor_RecalculateHash();
        }
        public void Editor_RecalculateHash()
        {
            _hashCode = HashUtility.GetDeterministicHashCode(_stringValue);
            EditorUtility.SetDirty(this);
        }
#endif
    }
    
    public static class HashUtility
    {
        public static int GetDeterministicHashCode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
                
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;
                
                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }
                
                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
