using System.Collections.Generic;
using System.Linq;
using YooAsset.Editor;

namespace PJR.Editor
{
    public static partial class CorrectAssetTools
    {
        public class ErrorReport
        {
            public class ErrorTerm
            {
                public string AssetPath;
                public string ErrorCodeSnip;
                public string ErrorCode;
                public CollectAssetInfo CollectAssetInfo;
            }

            public Dictionary<string, CollectAssetInfo> CollectAssetInfoMap =>
                _collectAssetInfoMap ??= new Dictionary<string, CollectAssetInfo>(1000);

            public List<ErrorTerm> ErrorTerms => _errorTerms ??= new List<ErrorTerm>(1000);

            public Dictionary<string, List<CollectAssetInfo>> AddressToCollectAssetInfos =>
                _addressToCollectAssetInfos ??= new Dictionary<string, List<CollectAssetInfo>>();


            private Dictionary<string, CollectAssetInfo> _collectAssetInfoMap;
            private List<ErrorTerm> _errorTerms;
            private Dictionary<string, List<CollectAssetInfo>> _addressToCollectAssetInfos;

            public bool ContainsKey(string key) => CollectAssetInfoMap.ContainsKey(key);

            public void Add(string key, CollectAssetInfo collectAssetInfo) =>
                CollectAssetInfoMap[key] = collectAssetInfo;

            public ErrorTerm AddErrorTerm(string assetPath, string errorCode, CollectAssetInfo collectAssetInfo,
                bool checkExist = false)
                => AddErrorTerm(assetPath, string.Empty, errorCode, collectAssetInfo, checkExist);

            public ErrorTerm AddErrorTerm(string assetPath, string errorCodeSnip, string errorCode,
                CollectAssetInfo collectAssetInfo, bool checkExist = false)
            {
                if (string.IsNullOrEmpty(assetPath))
                    return null;
                if (checkExist && ErrorTerms.Any(term => term.AssetPath == assetPath))
                    return null;
                var term = new ErrorTerm()
                {
                    AssetPath = assetPath,
                    ErrorCodeSnip = errorCodeSnip,
                    ErrorCode = errorCode,
                    CollectAssetInfo = collectAssetInfo
                };
                ErrorTerms.Add(term);
                return term;
            }
        }
    }
}