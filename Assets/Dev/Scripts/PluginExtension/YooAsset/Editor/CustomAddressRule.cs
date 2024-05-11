#if UNITY_EDITOR
using System.IO;
using YooAsset.Editor;

namespace PJR
{
    [DisplayName("定位地址: 文件名.后缀")]
    public class AddressByFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileName(data.AssetPath);
        }
    }

    [DisplayName("定位地址: 分组名_文件名.后缀")]
    public class AddressByGroupAndFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileName(data.AssetPath);
            return $"{data.GroupName}_{fileName}";
        }
    }

    [DisplayName("定位地址: 文件夹名_文件名.后缀")]
    public class AddressByFolderAndFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileName(data.AssetPath);
            FileInfo fileInfo = new FileInfo(data.AssetPath);
            return $"{fileInfo.Directory.Name}_{fileName}";
        }
    }
}
#endif