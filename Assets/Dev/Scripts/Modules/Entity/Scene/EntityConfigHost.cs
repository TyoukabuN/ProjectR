using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConfigHost : MonoBehaviour
{
    [LabelText("资源名字")]
    [ValidateInput("IsAssetNameValid","",InfoMessageType.Warning)]
    public string AssetName;
    public bool IsAssetNameValid(string assetName,ref string error)
    {
        error = "资源格式错误";
        if (string.IsNullOrEmpty(assetName))
        {
            error = "资源名字为空";
            return false;
        }
        if (assetName.IndexOf(".prefab") <= 0)
        {
            return false;
        }
        return true;
    }
}
