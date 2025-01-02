using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TImelineTest : MonoBehaviour
{
    const string k_ImagePath_C = "Assets/Plugins/com.unity.timeline@1.7.5/Editor/StyleSheets/Images/Icons/{0}.png";

    [Button]
    public void IconLoadTest()
    {
        var iconName = ResolveIcon("TimelineEditModeRippleON");
        var icon = AssetDatabase.LoadAssetAtPath<Texture>(iconName);
        Debug.Log(icon);
    }

    static string ResolveIcon(string icon)
    {
        return string.Format(k_ImagePath_C, icon);
        //return string.Format(k_ImagePath, icon);
    }
}
